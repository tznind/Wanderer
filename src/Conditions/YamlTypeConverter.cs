﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StarshipWanderer.Compilation;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Conditions
{
    public class YamlTypeConverter<T> : IYamlTypeConverter
    {
        private readonly TypeCollection _classesOfTypeT;

        public YamlTypeConverter()
        {
            _classesOfTypeT = Compiler.Instance.TypeFactory.Create<T>();
        }
        public bool Accepts(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>().Value;

            if (string.IsNullOrWhiteSpace(scalar))
                return null;

            string typeName;
            string[] generics = null;
            string[] args;

            if (scalar.Contains('<'))
            {
                //it's generics
                Regex pattern =  new Regex(@"([.\w]+)<([^>]*)>\(([^)]*)\)");
                var m = pattern.Match(scalar);

                if(!m.Success)
                    throw new ParseException($"Could not parse value '{scalar}' into a constructor call");

                typeName = m.Groups[1].Value;
                generics = SmoothSplit(m.Groups[2].Value);
                args = SmoothSplit(m.Groups[3].Value);
            }
            else
            {
                //its mundane
                Regex pattern =  new Regex(@"([.\w]+)\(([^)]*)\)");
                var m = pattern.Match(scalar);

                if(!m.Success)
                    throw new ParseException($"Could not parse value '{scalar}' into a constructor call");

                typeName = m.Groups[1].Value;
                args = SmoothSplit(m.Groups[2].Value);
            }


            Type adapterType = null;
            PropertyChain chain = null;
            
            //if we are dealing with something like "Recipient.Has"
            //then we need to follow the property chain
            if (typeName.Contains('.'))
            {
                if(typeof(T) != typeof(ICondition))
                    throw new NotSupportedException("PropertyChains are currently only supported for IConditions");

                chain = new PropertyChain(typeName, out string tail);
                typeName = tail;

                if(type.GenericTypeArguments.Length != 1)
                    throw new NotSupportedException($"Expected a fully hydrated ICondition with a known T Type but was '{type}'");

                adapterType = typeof(PropertyChainToConditionAdapter<>)
                    .MakeGenericType(type.GenericTypeArguments);
            }

            var constructor = new ConstructorCollection(_classesOfTypeT, typeName, generics);
            var builder = new InstanceBuilder();

            var instance = builder.Build(constructor, args);

            //if we need to wrap
            if (adapterType != null)
                instance = Activator.CreateInstance(adapterType,chain,instance);

            if (scalar.Trim().StartsWith("!"))
            {
                if (instance is ICondition i)
                    return Not<object>.Decorate(i);
                
                throw new NotSupportedException($"Not Operator '!' is only valid for IConditions");
            }

            return instance;

        }

        private string[] SmoothSplit(string value)
        {
            return
                value.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .ToArray();
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var condition = (ICondition) value;
            
            // reset of serialisation code
            emitter.Emit(new Scalar(condition.SerializeAsConstructorCall()));

        }
    }
}
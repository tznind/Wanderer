using System;
using System.Linq;
using System.Text.RegularExpressions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Compilation;
using TB.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Conditions
{
    public class YamlTypeConverter<T> : IYamlTypeConverter
    {
        private readonly TypeCollection _conditions;

        public YamlTypeConverter()
        {
            _conditions = Compiler.Instance.TypeFactory.Create<T>();
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
                Regex pattern =  new Regex(@"(\w+)<([^>]*)>\(([^)]*)\)");
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
                Regex pattern =  new Regex(@"(\w+)\(([^)]*)\)");
                var m = pattern.Match(scalar);

                if(!m.Success)
                    throw new ParseException($"Could not parse value '{scalar}' into a constructor call");

                typeName = m.Groups[1].Value;
                args = SmoothSplit(m.Groups[2].Value);
            }

            var constructor = new ConstructorCollection(_conditions, typeName, generics);
            var builder = new InstanceBuilder();

            return builder.Build(constructor, args);

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
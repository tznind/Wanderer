using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StarshipWanderer.Actions;
using StarshipWanderer.Compilation;
using StarshipWanderer.Effects;
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

            if (typeof(T) == typeof(ICondition) || typeof(T) == typeof(IEffect))
                return new Code(scalar);
            
            var found = _classesOfTypeT.FirstOrDefault(t => t.Name.Equals(scalar));

            if(found == null)
                throw new ParseException("Could not find Type " + scalar);
            
            return Activator.CreateInstance(found);
        }
        
        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var condition = (ICondition) value;
            
            // reset of serialisation code
            emitter.Emit(new Scalar(condition.CsharpCode));
        }
    }

    
}
using System;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Wanderer.Compilation
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

        public object ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>().Value;

            if (string.IsNullOrWhiteSpace(scalar))
                return null;

            if (typeof(T) == typeof(ICondition))
            {
                var conditionCodeType = typeof(ConditionCode<>).MakeGenericType(
                    type.GenericTypeArguments.Single());

                return Activator.CreateInstance(conditionCodeType, scalar);
            }

            if(typeof(T) == typeof(IEffect))
                return new EffectCode(scalar);

            if(typeof(T) == typeof(IFrameSource))
                return new FrameSourceCode(scalar);
            
            var found = _classesOfTypeT.FirstOrDefault(t => t.Name.Equals(scalar));

            if(found == null)
                throw new ParseException("Could not find Type " + scalar);
            
            return Activator.CreateInstance(found);
        }
        
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    
}
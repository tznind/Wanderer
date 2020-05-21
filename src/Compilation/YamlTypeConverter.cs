using System;
using System.Linq;
using Wanderer.Stats;
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

            return ParseScalar(scalar, type);
        }

        public object ParseScalar(string scalar, Type type)
        {
            if (string.IsNullOrWhiteSpace(scalar))
                return null;

            if(typeof(T) == typeof(Stat))
                return new Stat(scalar);
            
            var found = _classesOfTypeT.FirstOrDefault(t => t.Name.Equals(scalar));

            if(found == null)
                throw new ParseException($"Could not find Type '{scalar}' (either it does not exist or it is not a {typeof(T).Name})");
            
            return Activator.CreateInstance(found,true);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    
}
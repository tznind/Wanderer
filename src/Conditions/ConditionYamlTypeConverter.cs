using System;
using System.Linq;
using StarshipWanderer.Actors;
using TB.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Conditions
{
    public class ConditionYamlTypeConverter : IYamlTypeConverter
    {
        private readonly Type[] _conditions;

        public ConditionYamlTypeConverter()
        {
            _conditions = typeof(IConditionBase).Assembly.GetTypes()
                .Where(t => typeof(IConditionBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();
        }
        public bool Accepts(Type type)
        {
            return typeof(IConditionBase).IsAssignableFrom(type);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>();

            string[] split;
            split = scalar.Value.Split('(',',',')')
                .Where(s=>!string.IsNullOrWhiteSpace(s))
                .Select(s=>s.Trim())
                .ToArray();

            if (split.Length == 0)
                return null;

            var conditionType = _conditions.SingleOrDefault(c =>
                c.Name.Equals(split[0]) ||
                c.Name.StartsWith(split[0] + "Condition")); //use starts with to allow for generics

            if(conditionType == null)
                throw new YamlException($"Could not find ICondition called {split[0]}");
            
            //todo: one day we will have to support room / item based conditions too
            if (conditionType.ContainsGenericParameters)
                conditionType = conditionType.MakeGenericType(typeof(IActor));

            var constructor = conditionType
                .GetConstructors()
                .Where(c => c.GetParameters().Length == split.Length - 1)
                .ToArray();

            if(constructor.Length != 1)
                throw new YamlException($"Found {constructor.Length} constructors taking {split.Length - 1} parameters for Type {conditionType.FullName}");

            var c = constructor.Single();
            var p = c.GetParameters();
            var o = new object[p.Length];

            for (int i = 0; i < p.Length; i++)
            {
                o[i] = split[i + 1].To(p[i].ParameterType);
            }

            return c.Invoke(o);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var condition = (IConditionBase) value;
            
            // reset of serialisation code
            emitter.Emit(new Scalar(condition.SerializeAsConstructorCall()));

        }
    }
}
using System;
using System.Linq;
using System.Text.RegularExpressions;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Dialogues.Conditions;
using TB.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Dialogues
{
    public class DialogueConditionYamlTypeConverter : IYamlTypeConverter
    {
        private readonly Type[] _conditions;

        public DialogueConditionYamlTypeConverter()
        {
            _conditions = typeof(IDialogueCondition).Assembly.GetTypes()
                .Where(t => typeof(IDialogueCondition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();
        }
        public bool Accepts(Type type)
        {
            return typeof(IDialogueCondition).IsAssignableFrom(type);
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
                c.Name.Equals(split[0] + "Condition"));

            if(conditionType == null)
                throw new YamlException($"Could not find condition called {split[0]}");

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
            var condition = (IDialogueCondition) value;
            
            

            // reset of serialisation code
            emitter.Emit(new Scalar(condition.SerializeAsConstructorCall()));

        }
    }
}
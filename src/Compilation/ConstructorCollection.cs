using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using StarshipWanderer.Actors;
using YamlDotNet.Core;

namespace StarshipWanderer.Compilation
{
    public class ConstructorCollection : List<ConstructorInfo>
    {
        public Type Type { get; }

        public ConstructorCollection(Type type)
        {
            Type = type;
            AddRange(type.GetConstructors());
        }

        public ConstructorCollection(TypeCollection typeCollection, string typeName, string[] genericTemplatesIfAny)
        {
            genericTemplatesIfAny ??= new string[0];

            //if its Bob<T> then type name should be Bob`1
            if (!typeName.Contains('`') && genericTemplatesIfAny.Any())
                typeName = typeName + '`' + genericTemplatesIfAny.Length;
            
            var conditionType = typeCollection.SingleOrDefault(c =>
                c.Name.Equals(typeName));

            if (conditionType == null)
            {
                string suggestions = string.Join(Environment.NewLine,typeCollection
                    .Where(c => c.Name.Contains(typeName))
                    .Select(GetUserFriendlyName));
                if (!string.IsNullOrWhiteSpace(suggestions))
                    suggestions = "Did you mean:" + suggestions;

                throw new ParseException($"Could not find {typeCollection.BaseType} called {typeName}." + suggestions);
            }

            //if type is templated e.g. Bob<T>
            if (conditionType.ContainsGenericParameters)
            {
                var requiredGenerics = conditionType.GetGenericArguments();

                if(requiredGenerics.Length != genericTemplatesIfAny.Length)
                    throw new ParseException("Wrong number of generic arguments supplied");

                Type[] genericsValues = new Type[requiredGenerics.Length];

                var allTypes = Compiler.Instance.TypeFactory.Create(typeof(object), true, true);

                for (var index = 0; index < conditionType.GetGenericArguments().Length; index++)
                {
                    genericsValues[index] =
                        allTypes.Single(t => t.Name.Equals(genericTemplatesIfAny[index]));
                }

                conditionType = conditionType.MakeGenericType(genericsValues);
            }

            Type = conditionType;
            AddRange(Type.GetConstructors());
        }

        private string GetUserFriendlyName(Type arg)
        {
            Regex r = new Regex("(.*)`([0-9])$");
            var m = r.Match(arg.Name);
            if (m.Success)
            {
                int numberOfGenericParameters = int.Parse(m.Groups[2].Value);
                StringBuilder sb = new StringBuilder(m.Groups[1].Value);
                sb.Append('<');

                for (int i = 0; i < numberOfGenericParameters; i++)
                {
                    sb.Append("T" + (i + 1));
                    if (i + 1 < numberOfGenericParameters)
                        sb.Append(',');
                }
                sb.Append('>');
                return sb.ToString();
            }

            return arg.Name;
        }
    }
}

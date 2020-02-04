using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using StarshipWanderer.Actors;
using YamlDotNet.Core;

namespace StarshipWanderer.Compilation
{
    class ConstructorCollection : List<ConstructorInfo>
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

            if(conditionType == null)
                throw new ParseException($"Could not find {typeCollection.BaseType} called {typeName}");

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
    }
}

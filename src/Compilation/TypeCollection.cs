using System;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Compilation
{
    public class TypeCollection : List<Type>
    {
        /// <summary>
        /// The base Type from which all the Types in the collection
        /// should be derived
        /// </summary>
        public Type BaseType { get; }

        public TypeCollection(Type baseType,params Type[] initialCollection)
        {
            BaseType = baseType;
            AddRange(initialCollection);
        }

        public Type GetTypeNamed(string name)
        {
             var match = this.Where(t=>t.Name.Equals(name)).ToArray();

             if(match.Length != 1)
                 throw new Exception($"Found {match} types called '{name}' (with BaseType {BaseType})");

            return match[0];
        }
    }
}
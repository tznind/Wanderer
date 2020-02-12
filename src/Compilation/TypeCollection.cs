using System;
using System.Collections.Generic;

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
    }
}
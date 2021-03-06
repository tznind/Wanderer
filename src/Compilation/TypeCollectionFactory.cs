﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wanderer.Compilation
{
    public class TypeCollectionFactory
    {
        private readonly Assembly[] _assemblies;

        Lazy<Type[]> _getAllTypesLazy;

        public TypeCollectionFactory(params Assembly[]  assemblies)
        {
            _assemblies = assemblies;
            _getAllTypesLazy = new Lazy<Type[]>(()=>_assemblies.SelectMany(a=>a.GetTypes()).ToArray());
        }
        
        public TypeCollection Create<T>(bool includeAbstract = false, bool includeInsterface = false)
        {
            return Create(typeof(T), includeAbstract, includeInsterface);
        }

        public TypeCollection Create(Type baseType, bool includeAbstract, bool includeInterface)
        {
            return new TypeCollection(baseType, _getAllTypesLazy.Value
                .Where(t => baseType.IsAssignableFrom(t) && (!t.IsInterface || includeInterface) && (!t.IsAbstract || includeAbstract))
                .ToArray());
        }
    }
}

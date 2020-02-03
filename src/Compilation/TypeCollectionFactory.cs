using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarshipWanderer.Compilation
{
    public class TypeCollectionFactory
    {
        private readonly Assembly[] _assemblies;

        public TypeCollectionFactory(params Assembly[]  assemblies)
        {
            _assemblies = assemblies;
        }
        
        public TypeCollection Create<T>(bool includeAbstract = false, bool includeInsterface = false)
        {
            return Create(typeof(T), includeAbstract, includeInsterface);
        }

        public TypeCollection Create(Type baseType, bool includeAbstract, bool includeInterface)
        {
            return new TypeCollection(baseType,_assemblies.SelectMany(a=>a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && (!t.IsInterface || includeInterface) && (!t.IsAbstract || includeAbstract))
                .ToArray());
        }
    }
}

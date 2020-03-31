using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Adjectives
{
    public static class ListExtensions
    {
        public static bool AreIdentical<T>(this List<T> first, List<T> second) where T : IAreIdentical
        {
            if (second == null)
                return false;

            if (first == second)
                return true;

            if (first.Count != second.Count)
                return false;
            
            return first.TrueForAll(e=>second.Any(o=>e.AreIdentical(e)));
        }
    }
}
using System.Collections.Generic;

namespace StarshipWanderer.Adjectives
{
    public class Collection<T> : List<T>
    {
        public bool AreEqual(Collection<T> other)
        {
            if (this == other)
                return true;

            if (this.Count != other.Count)
                return false;

            return TrueForAll(other.Contains);
        }
    }
}
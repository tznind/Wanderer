using System.Collections.Generic;

namespace StarshipWanderer.Adjectives
{
    public class SwCollection<T> : List<T>, ISwCollection<T>
    {
        public virtual bool AreIdentical(ISwCollection<T> other)
        {
            if (other == null)
                return false;

            if (this == other)
                return true;

            if (this.Count != other.Count)
                return false;

            return TrueForAll(other.Contains);
        }
    }
}
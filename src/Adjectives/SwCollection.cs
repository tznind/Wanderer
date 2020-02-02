using System.Collections.Generic;
using System.Linq;

namespace StarshipWanderer.Adjectives
{
    public class SwCollection<T> : List<T>, ISwCollection<T> where T : IAreIdentical<T>
    {
        public virtual bool AreIdentical(ISwCollection<T> other)
        {
            if (other == null)
                return false;

            if (this == other)
                return true;

            if (this.Count != other.Count)
                return false;
            
            return TrueForAll(e=>other.Any(o=>o.AreIdentical(e)));
        }

        public void PruneNulls()
        {
            RemoveAll(v => v == null);
        }
    }
}
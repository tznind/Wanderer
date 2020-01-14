using System.Linq;

namespace StarshipWanderer.Adjectives
{
    public class AdjectiveCollection : SwCollection<IAdjective>,IAdjectiveCollection
    {
        public override bool AreIdentical(ISwCollection<IAdjective> other)
        {
            
            if (other == null)
                return false;

            if (this == other)
                return true;

            if (this.Count != other.Count)
                return false;

            //They won't Equal but they will be identical adjectives
            return TrueForAll(e=>other.Any(o=>o.AreIdentical(e)));
        }
    }
}
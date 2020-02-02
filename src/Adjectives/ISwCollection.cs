using System.Collections.Generic;

namespace StarshipWanderer.Adjectives
{
    public interface ISwCollection<T>: IList<T>
    {
        bool AreIdentical(ISwCollection<T> other);

        void PruneNulls();
    }
}
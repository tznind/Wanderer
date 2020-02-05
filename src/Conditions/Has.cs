using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Conditions
{
    public class Has<T1, T2> : ICondition<T1> where T1 : IHasStats where T2 : IAdjective
    {
        public bool IncludeItems { get; set; }

        public Has(bool includeItems)
        {
            IncludeItems = includeItems;
        }
        public bool IsMet(T1 forTarget)
        {
            if (forTarget is IActor a)
                return a.Has<T2>(IncludeItems);

            if (forTarget is IPlace p)
                return p.Has<T2>();

            if (forTarget is IItem i)
                return i.Has<T2>(null);

            return forTarget.Adjectives.Any(j => j is T2);
        }

        public virtual string? SerializeAsConstructorCall()
        {
            return $"Has<{typeof(T1).Name},{typeof(T2).Name}>({IncludeItems})";
        }

    }
}
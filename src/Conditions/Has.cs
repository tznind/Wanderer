using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Conditions
{
    public class Has<T> : ICondition<IHasStats> where T : IAdjective
    {
        public bool IncludeItems { get; set; }

        public Has(bool includeItems)
        {
            IncludeItems = includeItems;
        }
        public bool IsMet(IHasStats forTarget)
        {
            if (forTarget is IActor a)
                return a.Has<T>(IncludeItems);

            if (forTarget is IPlace p)
                return p.Has<T>();

            if (forTarget is IItem i)
                return i.Has<T>(null);

            return forTarget.Adjectives.Any(j => j is T);
        }

        public virtual string? SerializeAsConstructorCall()
        {
            return $"Has<{typeof(T).Name}>({IncludeItems})";
        }
    }
}
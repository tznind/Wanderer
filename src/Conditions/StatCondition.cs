using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

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

    public class StatCondition<T> : ICondition<IHasStats> where  T: IHasStats
    {
        public Stat ToCheck { get; set; }
        public Comparison Comparison { get; set; }
        public double Value { get; set; }

        public StatCondition(Stat toCheck,Comparison comparison, double value)
        {
            ToCheck = toCheck;
            Comparison = comparison;
            Value = value;
            
        }

        public bool IsMet(T owner)
        {
            return Comparison.IsMet(owner.BaseStats[ToCheck], Value);
        }

        public bool IsMet(IHasStats forTarget)
        {
            return IsMet((T)forTarget);
        }

        public string? SerializeAsConstructorCall()
        {
            return $"{GetType().Name}<{typeof(T).Name}>({ToCheck},{Comparison},{Value})";
        }

        public override string ToString()
        {
            return $"{ToCheck} {Comparison} {Value}";
        }
    }
}
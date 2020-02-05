using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
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
using System;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
    public abstract class StatCondition
    {
        public Stat ToCheck { get; set; }
        public Comparison Comparison { get; set; }
        public int Value { get; set; }

        public StatCondition(Stat toCheck,Comparison comparison, int value)
        {
            ToCheck = toCheck;
            Comparison = comparison;
            Value = value;
            
        }

        public bool IsMet(StatsCollection stats)
        {
            switch (Comparison)
            {
                case Comparison.LessThan:
                    return stats[ToCheck] < Value;
                case Comparison.GreaterThanOrEqual:
                    return stats[ToCheck] >= Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
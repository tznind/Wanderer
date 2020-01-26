using System;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
    public abstract class StatCondition
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

        public bool IsMet(StatsCollection stats)
        {
            return Comparison.IsMet(stats[ToCheck], Value);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer
{
    public abstract class HasStats : IHasStats
    {
        public string Name { get; set; }

        public HashSet<IAdjective> Adjectives { get; set; } = new HashSet<IAdjective>();
        public HashSet<IAction> BaseActions { get; set; } = new HashSet<IAction>();
        public StatsCollection BaseStats { get; set; } = new StatsCollection();
        public HashSet<IBehaviour> BaseBehaviours { get; set; } = new HashSet<IBehaviour>();
        
        public abstract StatsCollection GetFinalStats();

        public abstract IEnumerable<IAction> GetFinalActions();

        public abstract IEnumerable<IBehaviour> GetFinalBehaviours();
        
        public override string ToString()
        {
            return (string.Join(" ", Adjectives.Where(a=>a.IsPrefix)) + " " + (Name ?? "Unnamed Object")).Trim();
        }
    }
}
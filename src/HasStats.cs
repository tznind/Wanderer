using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
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
        
        public abstract StatsCollection GetFinalStats(IActor forActor);

        public abstract IEnumerable<IAction> GetFinalActions(IActor forActor);

        public abstract IEnumerable<IBehaviour> GetFinalBehaviours(IActor forActor);
        
        public override string ToString()
        {
            return (string.Join(" ", Adjectives.Where(a=>a.IsPrefix)) + " " + (Name ?? "Unnamed Object")).Trim();
        }
    }
}
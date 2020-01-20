using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public abstract class Adjective : HasStats,IAdjective
    {
        public IHasStats Owner { get; set; }

        public bool IsPrefix { get; set; } = true;
        
        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        protected Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }
        
        public override IActionCollection GetFinalActions(IActor forActor)
        {
            return BaseActions;
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            return BaseStats;
        }
        public override IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            return BaseBehaviours;
        }
        
        public abstract IEnumerable<string> GetDescription();

        
    }

    class Giant : Adjective
    {
        public Giant(IHasStats owner) : base(owner)
        {
            BaseStats[Stat.Fight] = 20;
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Fight better";
        }
    }
}
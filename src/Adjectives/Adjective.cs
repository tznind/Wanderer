using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public abstract class Adjective : IAdjective
    {
        public IHasStats Owner { get; set; }

        public bool IsPrefix { get; set; } = true;

        public string Name { get; set; }

        /// <summary>
        /// Interface member, do not use. Sub adjectives seem like a really bad idea
        /// </summary>
        public IAdjectiveCollection Adjectives { get; set; }

        public IActionCollection BaseActions { get; set; } = new ActionCollection();

        public StatsCollection BaseStats { get; set; } = new StatsCollection();
        public IBehaviourCollection BaseBehaviours { get; set; } = new BehaviourCollection();
        
        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        protected Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }
        
        public virtual IActionCollection GetFinalActions(IActor forActor)
        {
            return BaseActions;
        }

        public virtual StatsCollection GetFinalStats(IActor forActor)
        {
            return BaseStats;
        }
        public virtual IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            return BaseBehaviours;
        }


        public override string ToString()
        {
            return Name;
        }
        
        public abstract IEnumerable<string> GetDescription();

        
    }
}
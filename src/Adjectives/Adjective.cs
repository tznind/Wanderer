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

        public string Name { get; set; }

        /// <summary>
        /// Interface member, do not use. Sub adjectives seem like a really bad idea
        /// </summary>
        public HashSet<IAdjective> Adjectives { get; set; }

        public HashSet<IAction> BaseActions { get; set; } = new HashSet<IAction>();

        public StatsCollection BaseStats { get; set; } = new StatsCollection();
        public HashSet<IBehaviour> BaseBehaviours { get; set; } = new HashSet<IBehaviour>();
        
        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        protected Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }
        
        public virtual IEnumerable<IAction> GetFinalActions()
        {
            return BaseActions;
        }

        public virtual StatsCollection GetFinalStats()
        {
            return BaseStats;
        }
        public virtual IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return BaseBehaviours;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual bool IsActive()
        {
            return true;
        }
    }
}
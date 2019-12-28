using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public abstract class Adjective : IAdjective
    {
        public IActor Owner { get; set; }
        
        public StatsCollection Modifiers { get; } = new StatsCollection();
        public List<IBehaviour> Behaviours { get; } = new List<IBehaviour>();
        public List<IAction> Actions { get; set; } = new List<IAction>();

        protected Adjective(IActor owner)
        {
            Owner = owner;
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
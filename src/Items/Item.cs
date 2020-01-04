using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public class Item : HasStats,IItem
    {
        public IActor OwnerIfAny { get; set; }

        public Item(string name)
        {
            Name = name;
        }
        
        public override StatsCollection GetFinalStats()
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives.Where(a=>a.IsActive())) 
                clone.Add(adjective.GetFinalStats());

            return clone;
        }

        public override IEnumerable<IAction> GetFinalActions()
        {
            return BaseActions.Union(Adjectives.SelectMany(a => a.GetFinalActions()));
        }

        public override IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours()));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
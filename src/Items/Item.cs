using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public class Item : HasStats,IItem
    {
        public IActor OwnerIfAny { get; set; }
        public void Drop(IUserinterface ui, IPlace dropLocation, Guid round)
        {
            if(OwnerIfAny == null)
                throw new NoOwnerException("Item has no owner so cannot be dropped");

            if (dropLocation == null)
                dropLocation = OwnerIfAny.CurrentLocation;

            //remove us from the owner
            OwnerIfAny.Items.Remove(this);
            //add us to the room
            dropLocation.Items.Add(this);

            //log it
            ui.Log.Info($"{OwnerIfAny} dropped {this}", round);
            
            //clear owner status
            OwnerIfAny = null;
        }

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public class Item : HasStats,IItem
    {
        /// <inheritdoc/>
        public IItemSlot Slot { get; set; }

        public bool IsEquipped { get; set; }

        /// <summary>
        /// Set on items that should have been removed from the game to prevent
        /// later reprocessing e.g. when one behaviour deletes another behaviours
        /// owner before that behaviour has itself resolved
        /// </summary>
        public bool IsErased { get; set; } = false;


        public List<ICondition<IActor>> Require { get; set; } = new List<ICondition<IActor>>();


        public void Drop(IUserinterface ui, IActor owner, Guid round)
        {
            //remove us from the owner
            owner.Items.Remove(this);
            //add us to the room
            owner.CurrentLocation.Items.Add(this);
            
            //hes not wearing it anymore
            IsEquipped = false;

            //log it
            ui.Log.Info(new LogEntry($"{owner} dropped {this}", round,owner));
        }

        public bool Has<T>(IActor owner) where T : IAdjective
        {
            return Adjectives.Any(a => a is T);
        }

        public bool Has<T>(IActor owner, Func<T, bool> condition) where T : IAdjective
        {
            return Adjectives.Any(a => a is T t && condition(t));
        }

        public bool CanUse(IActor actor)
        {
            if (IsErased)
                return false;

            return Require.All(c => c.IsMet(actor));
        }


        public Item(string name)
        {
            Name = name;
        }
        
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return new StatsCollection();

            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Add(adjective.GetFinalStats(forActor));

            return clone;
        }

        

        public override IActionCollection GetFinalActions(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return new ActionCollection();

            return new ActionCollection(BaseActions.Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor))));
        }

        public override IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return new BehaviourCollection();

            return new BehaviourCollection(BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours(forActor))));
        }

        public override string ToString()
        {
            return IsEquipped ? "[E]" + base.ToString() : base.ToString();
        }
    }
}
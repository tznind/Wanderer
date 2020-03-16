using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Rooms;
using Wanderer.Stats;

namespace Wanderer.Items
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


        public List<ICondition<IHasStats>> Require { get; set; } = new List<ICondition<IHasStats>>();


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

        
        public bool CanUse(IActor actor,out string reason)
        {
            if (IsErased)
            {
                reason = "Item no longer exists";
                return false;
            }
            
            if(!Require.All(c => c.IsMet(actor.CurrentLocation.World,actor)))
            {
                reason = "Item requirements not met:" + string.Join(Environment.NewLine,Require.Select(r=>r.ToString()));
                return false;
            }


            reason = null;
            return true;
        }


        public Item(string name)
        {
            Name = name;
        }

        public bool RequirementsMet(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return false;

            //it has unique conditions that are not met yet
            return Require.All(r => r.IsMet(forActor.CurrentLocation.World,forActor));
        }
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            if(new StackTrace().FrameCount > 1000)
                throw new Exception("Looks like your about to have a stack overflow.  Are you calling GetFinalStats on an item whose requirements include a Stat check? Maybe switch to BaseStats");

            if(!RequirementsMet(forActor))
                return new StatsCollection();

            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Increase(adjective.GetFinalStats(forActor));

            foreach(var adjective in Adjectives)
                clone = adjective.Modify(clone);
                
            return clone;
        }

        

        public override IActionCollection GetFinalActions(IActor forActor)
        {
            //if it requires equipping
            if(!RequirementsMet(forActor))
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
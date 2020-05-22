using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Rooms;
using Wanderer.Stats;
using Wanderer.Systems;

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


        public List<ICondition> Require { get; set; } = new List<ICondition>();


        public List<ICondition> EquipRequire { get; set; } = new List<ICondition>();


        public List<ICondition> UnEquipRequire { get; set; } = new List<ICondition>();

        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected Item()
        {

        }

        public Item(string name)
        {
            Name = name;
        }

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
            
            if(!Require.All(c => c.IsMet(actor.CurrentLocation.World,GetSystemArgs(actor))))
            {
                reason = "Item requirements not met:" + string.Join(Environment.NewLine,Require.Select(r=>r.ToString()));
                return false;
            }

            reason = null;
            return true;
        }

        public bool CanEquip(IActor actor, out string reason)
        {
            //already equipped, dead etc
            if (IsEquipped)
            {
                reason = "Already equipped";
                return false;
            }

            if (Slot == null)
            {
                reason = "Item cannot be equipped";
                return false;
            }

            if (!actor.AvailableSlots.ContainsKey(Slot.Name))
            {
                reason = $"You do not have a {Slot.Name} slot";
                return false;
            }

            // Does the actor already have too many filled slots
            var alreadyWearing = actor.Items.Where(i => i.IsEquipped && i.Slot != null && i.Slot.Name == Slot.Name);
            var alreadyWearingCount = alreadyWearing.Sum(i => i.Slot.NumberRequired);

            if (actor.AvailableSlots[Slot.Name] < Slot.NumberRequired + alreadyWearingCount)
            {
                reason = $"You do not have enough free {Slot.Name} slots";
                return false;
            }
            
            if(!EquipRequire.All(c => c.IsMet(actor.CurrentLocation.World,GetSystemArgs(actor))))
            {
                reason = "Item requirements not met:" + string.Join(Environment.NewLine,EquipRequire.Select(r=>r.ToString()));
                return false;
            }

            reason = null;
            return true;
        }
        public bool CanUnEquip(IActor actor, out string reason)
        {
            //already not even equiped!
            if (!IsEquipped)
            {
                reason = "Already not equiped";
                return false;
            }
            
            if(!UnEquipRequire.All(c => c.IsMet(actor.CurrentLocation.World,GetSystemArgs(actor))))
            {
                reason = "Item requirements not met:" + string.Join(Environment.NewLine,UnEquipRequire.Select(r=>r.ToString()));
                return false;
            }

            reason = null;
            return true;
        }

        public IInjurySystem GetBestInjurySystem(IActor forActor)
        {
            //get an injury system from the FightAction of a weapon
            var system = GetFinalActions(forActor)
                .OfType<FightAction>()
                .FirstOrDefault(i=>i.InjurySystem != null)
                ?.InjurySystem;

            //or the injury system of the weapon itself
            return system ?? InjurySystem;
        }


        public bool RequirementsMet(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return false;

            //it has unique conditions that are not met yet
            return Require.All(r => r.IsMet(forActor.CurrentLocation.World,GetSystemArgs(forActor)));
        }

        private SystemArgs GetSystemArgs(IActor forActor)
        {
            return new SystemArgs(forActor.CurrentLocation.World,null,0,forActor,forActor,Guid.Empty);
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

        

        public override List<IAction> GetFinalActions(IActor forActor)
        {
            
            List<IAction> alwaysActions = new List<IAction>
            {
                new GiveAction(this),
                new DropAction(this)
            };

            //if it requires equipping
            if(!RequirementsMet(forActor))
                return new List<IAction>(alwaysActions);

            return new List<IAction>(alwaysActions
                    .Union(BaseActions)
                    .Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor)))
                );
        }

        public override List<IBehaviour> GetFinalBehaviours(IActor forActor)
        {
            //if it requires equipping
            if(Slot != null && !IsEquipped)
                return new List<IBehaviour>();

            return new List<IBehaviour>(BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours(forActor))));
        }

        public override string ToString()
        {
            return IsEquipped ? "[E]" + base.ToString() : base.ToString();
        }
    }
}
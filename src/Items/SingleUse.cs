using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Adjectives;
using Wanderer.Behaviours;

namespace Wanderer.Items
{
    public class SingleUse : Adjective, IBehaviour
    {
        public IItem OwnerItem { get; set; }

        public SingleUse(IItem owner) : base(owner)
        {
            OwnerItem = owner;
            BaseBehaviours.Add(this);
        }
        
        public override IEnumerable<string> GetDescription()
        {
            yield return "Can only be used once";
        }

        public void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
        }

        public void OnPop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            //if an action is pushed onto the stack that comes from the owned item
            if (OwnerItem.GetFinalActions(frame.PerformedBy).Any(a=>ReferenceEquals(a,frame.Action)))
                //erase it from existence
                frame.PerformedBy.CurrentLocation.World.Erase(OwnerItem);
        }

        public void OnRoundEnding(IUserinterface ui, Guid round)
        {
        }

        public bool AreIdentical(IBehaviour other)
        {
            return other is IAdjective oa && this.AreIdentical(oa);
        }
    }
}
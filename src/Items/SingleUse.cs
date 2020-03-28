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
            IsPrefix = false;
        }
        public void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
        }

        public void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            //if an action is pushed onto the stack that comes from the owned item
            if (OwnerItem.GetFinalActions(frame.PerformedBy).Any(a=>ReferenceEquals(a,frame.Action)))
            {
                //stack of 2+ just decrements the stack
                if(OwnerItem is IItemStack s && s.StackSize > 1)
                {
                    s.StackSize --;
                    return;
                }
                
                //erase it from existence
                frame.PerformedBy.CurrentLocation.World.Erase(OwnerItem);
            }
        }

        public void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
        }

        public bool AreIdentical(IBehaviour other)
        {
            return other is SingleUse;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class DropAction : Action
    {
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Drop","Select an item to drop",out IItem toDrop, GetTargets(actor),-10))
                stack.Push(new DropFrame(actor,this,toDrop,- GetItemWorthInAttitude(actor,toDrop)));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (DropFrame) frame;

            //if we still have the item we should drop it
            if(f.PerformedBy.Items.Contains(f.ToDrop))
                f.ToDrop.Drop(ui, f.PerformedBy,stack.Round);
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
        private IItem[] GetTargets(IActor performer)
        {
            return performer.Items.ToArray();
        }

        private double GetItemWorthInAttitude(IActor dropper, IItem toDrop)
        {
            //value of item is total value of the item to the recipient
            return toDrop.GetFinalStats(dropper)[Stat.Value];
        }
    }
}

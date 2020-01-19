using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class DropAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Drop","Select an item to drop",out IItem toDrop, actor.Items.ToArray(),-10))
                stack.Push(new DropFrame(actor,this,toDrop,- GetItemWorthInAttitude(actor,toDrop)));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (DropFrame) frame;

            //if we still have the item we should drop it
            if(f.PerformedBy.Items.Contains(f.ToDrop))
                f.ToDrop.Drop(ui, f.PerformedBy,stack.Round);
        }
        private double GetItemWorthInAttitude(IActor dropper, IItem toDrop)
        {
            //value of item is total value of the item to the recipient
            return toDrop.GetFinalStats(dropper)[Stat.Value];
        }
    }
}

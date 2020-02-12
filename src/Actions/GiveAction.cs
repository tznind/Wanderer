using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class GiveAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Give","Select an item to give",out IItem toGive, GetTargets(actor),-10))
                if(actor.Decide(ui,"To whom",$"Select who to give {toGive}",out IActor toGiveTo, actor.GetCurrentLocationSiblings(false),10))
                    stack.Push(new GiveFrame(actor,this,toGive,toGiveTo,GetItemWorthInAttitude(actor,toGive,toGiveTo)));
        }

        private double GetItemWorthInAttitude(IActor giver, IItem toGive, IActor toGiveTo)
        {
            //value of item is total value of the item if the recipient was holding it
            return toGive.GetFinalStats(toGiveTo)[Stat.Value];
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (GiveFrame) frame;

            //if we still have the item we should drop it
            if (f.PerformedBy.Items.Contains(f.ItemToGive) && ! f.ItemToGive.IsErased)
            {
                f.ItemToGive.IsEquipped = false;
                f.PerformedBy.Items.Remove(f.ItemToGive);
                f.TargetIfAny.Items.Add(f.ItemToGive);
                ui.Log.Info(new LogEntry($"{f.PerformedBy} gave {f.ItemToGive} to {f.TargetIfAny}",stack.Round,f.PerformedBy));
            }
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        private IItem[] GetTargets(IActor performer)
        {
            return performer.Items.ToArray();
        }
    }
}
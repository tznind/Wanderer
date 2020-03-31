using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class GiveAction : Action
    {
        public GiveAction(IHasStats owner):base(owner)
        {
            HotKey = 'g';
        }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(Owner is IItem toGive || actor.Decide(ui,"Give","Select an item to give",out toGive, GetTargets(actor).Cast<IItem>().ToArray(),-10))
                if(actor.Decide(ui,"To whom",$"Select who to give {toGive}",out IActor toGiveTo, actor.GetCurrentLocationSiblings(false),10))
                    stack.Push(new GiveFrame(actor,this,toGive,toGiveTo,GetItemWorthInAttitude(actor,toGive,toGiveTo)));
        }

        private double GetItemWorthInAttitude(IActor giver, IItem toGive, IActor toGiveTo)
        {
            //value of item is total value of the item if the recipient was holding it
            return toGive.GetFinalStats(toGiveTo)[Stat.Value];
        }

        protected override void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (GiveFrame) frame;

            //if we still have the item we should drop it
            if (f.PerformedBy.Items.Contains(f.ItemToGive) && ! f.ItemToGive.IsErased)
            {
                f.ItemToGive.IsEquipped = false;
                f.PerformedBy.Items.Remove(f.ItemToGive);
                ((IActor)f.TargetIfAny).Items.Add(f.ItemToGive);
                ui.Log.Info(new LogEntry($"{f.PerformedBy} gave {f.ItemToGive} to {f.TargetIfAny}",stack.Round,f.PerformedBy));
            }
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return performer.Items.ToArray();
        }
    }
}
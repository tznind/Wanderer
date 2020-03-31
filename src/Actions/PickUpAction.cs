using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Actions
{
    public class PickUpAction : Action
    {
        public PickUpAction(IHasStats owner) : base(owner)
        {
            HotKey = 'p';
        }

        public IItem PrimeWithTarget { get; set; }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            var chosen = PrimeWithTarget;

            if(chosen != null || actor.Decide(ui,"Pick Up", null, out chosen, GetTargets(actor).Cast<IItem>().ToArray() ,0))
                stack.Push(new PickUpFrame(actor,this,chosen,actor.CurrentLocation,0));
        }

        protected override void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (PickUpFrame)frame;
            PrimeWithTarget = null;

            if (f.FromRoom.Items.Contains(f.Item))
            {
                //remove it from the location and give to player
                f.FromRoom.Items.Remove(f.Item);
                f.PerformedBy.Items.Add(f.Item);

                ui.Log.Info(new LogEntry($"{f.PerformedBy} picked up {f.Item}",stack.Round,f.PerformedBy));
            }
            else
            {
                ui.Log.Info(new LogEntry($"{f.PerformedBy} attempted to pick up {f.Item} but was too slow",stack.Round,f.PerformedBy));
            }
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return performer.CurrentLocation.Items.ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
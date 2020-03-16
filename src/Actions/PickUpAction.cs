using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Actions
{
    public class PickUpAction : Action
    {
        private PickUpAction():base(null)
        {
            
        }
        public PickUpAction(IHasStats owner) : base(owner)
        {
        }

        public override char HotKey => 'p';

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {            
            if(actor.Decide(ui,"Pick Up", null, out IItem chosen, GetTargets(actor),0))
                stack.Push(new PickUpFrame(actor,this,chosen,actor.CurrentLocation,0));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (PickUpFrame)frame;

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

        private IItem[] GetTargets(IActor performer)
        {
            return performer.CurrentLocation.Items.ToArray();
        }
    }
}
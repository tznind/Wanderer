using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class PickUpAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {            
            if(actor.Decide(ui,"Pick Up", null, out IItem chosen, actor.CurrentLocation.Items.ToArray(),0))
                stack.Push(new PickUpFrame(actor,this,chosen,actor.CurrentLocation,0));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (PickUpFrame)frame;

            if (f.FromPlace.Items.Contains(f.Item))
            {
                //remove it from the location and give to player
                f.FromPlace.Items.Remove(f.Item);
                f.PerformedBy.Items.Add(f.Item);

                ui.Log.Info(new LogEntry($"{f.PerformedBy} picked up {f.Item}",stack.Round,f.PerformedBy));
            }
            else
            {
                ui.Log.Info(new LogEntry($"{f.PerformedBy} attempted to pick up {f.Item} but was too slow",stack.Round,f.PerformedBy));
            }
        }
    }
}
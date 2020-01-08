using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Actions
{
    public class CoerceAction : Action
    {
        public CoerceAction()
        {
            Attitude = -10;
        }

        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            //pick a target 
            if(actor.Decide(ui,"Coerce Target", null, out Npc toCoerce, actor.GetCurrentLocationSiblings().OfType<Npc>().ToArray(),Attitude))
                //pick an action to perform
                if (actor.Decide(ui,"Coerce Action", $"Pick an action you want {toCoerce} to perform", out IAction actionToCoerce,toCoerce.GetFinalActions(toCoerce).ToArray(),0))
                    stack.Push(new CoerceFrame(actor, this, toCoerce, actionToCoerce,ui));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = frame as CoerceFrame;
            f.ToCoerce.NextAction = f;
            f.ToCoerce.Adjectives.Add(new Coerced(f.ToCoerce));

            ui.Log.Info(new LogEntry($"{f.PerformedBy} coerced {f.ToCoerce} to perform {f.CoerceAction.Name}", stack.Round,frame.PerformedBy));
        }
    }
}

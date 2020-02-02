using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Actions
{
    public class CoerceAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            //pick a target 
            if(actor.Decide(ui,"Coerce Target", null, out Npc toCoerce, GetTargets(actor),-20))
                //pick an action to perform
                if (actor.Decide(ui, "Coerce Action", $"Pick an action you want {toCoerce} to perform",
                    out IAction actionToCoerce,
                    toCoerce.GetFinalActions(toCoerce).Where(a => a.HasTargets(toCoerce)).ToArray(), 0))
                    stack.Push(new CoerceFrame(actor, this, toCoerce, actionToCoerce,ui,-10));
        }


        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (CoerceFrame)frame ;
            ((Npc)f.TargetIfAny).NextAction = f;
            f.TargetIfAny.Adjectives.Add(new Coerced(f.TargetIfAny));

            ui.Log.Info(new LogEntry($"{f.PerformedBy} coerced {f.TargetIfAny} to perform {f.CoerceAction.Name}", stack.Round,frame.PerformedBy));
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
        private Npc[] GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(false).OfType<Npc>().ToArray();
        }
    }
}

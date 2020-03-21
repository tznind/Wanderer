using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;

namespace Wanderer.Actions.Coercion
{
    public class CoerceAction : Action
    {
        
        private CoerceAction() : base(null)
        {
        }
        public CoerceAction(IHasStats owner) : base(owner)
        {
        }
        public override char HotKey => 'c';

        public IActor PrimeWithTarget { get; internal set; }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            IActor toCoerce = PrimeWithTarget;

            //pick a target 
            if(toCoerce != null || actor.Decide(ui,"Coerce Target", null, out toCoerce, GetTargets(actor).Cast<Npc>().ToArray(),-20))
                //pick an action to perform
                if (actor.Decide(ui, "Coerce Action", $"Pick an action you want {toCoerce} to perform",
                    out IAction actionToCoerce,
                    toCoerce.GetFinalActions(toCoerce).Where(a => a.HasTargets(toCoerce)).ToArray(), 0))
                    stack.Push(new CoerceFrame(actor, this, (Npc)toCoerce, actionToCoerce, ui,
                        actor.CurrentLocation.World.NegotiationSystems.First(), -10));
        }


        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (CoerceFrame)frame ;
            ((Npc)f.TargetIfAny).NextAction = f;
            f.TargetIfAny.Adjectives.Add(new Coerced(f));

            ui.Log.Info(new LogEntry($"{f.PerformedBy} coerced {f.TargetIfAny} to perform {f.CoerceAction.Name}", stack.Round,frame.PerformedBy));
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(false).OfType<Npc>().ToArray();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}

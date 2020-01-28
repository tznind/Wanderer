using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Behaviours
{
    internal class ForbidAction : Action
    {
        private readonly Frame _toForbid;

        public ForbidAction(Frame toForbid)
        {
            _toForbid = toForbid;
        }

        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,0));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            _toForbid.Cancelled = CancellationStatus.Cancelled;
            ui.Log.Info(new LogEntry(
                $"{frame.PerformedBy} prevented {_toForbid.PerformedBy} from performing action {_toForbid.Action.Name}",
                stack.Round, _toForbid.PerformedBy));
        }

        public override bool HasTargets(IActor performer)
        {
            return true;
        }
    }
}
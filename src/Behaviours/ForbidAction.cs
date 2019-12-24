using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Behaviours
{
    internal class ForbidAction : Action
    {
        private readonly IAction _toForbid;

        public ForbidAction(IAction toForbid, IActor performedBy):base(null,performedBy)
        {
            _toForbid = toForbid;
        }


        public override void Push(IUserinterface ui, ActionStack stack)
        {
            
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            _toForbid.Cancelled = CancellationStatus.Cancelled;
            ui.ShowMessage("Forbidden",$"{PerformedBy} prevented {_toForbid.PerformedBy} from performing action {_toForbid.Name}");
        }
    }
}
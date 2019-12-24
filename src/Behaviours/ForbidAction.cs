using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    internal class ForbidAction : Action
    {
        private readonly IAction _toForbid;

        public ForbidAction(IAction toForbid):base(null)
        {
            _toForbid = toForbid;
        }


        public override void Push(IUserinterface ui, ActionStack stack)
        {
            
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            _toForbid.Cancelled = CancellationStatus.Cancelled;
        }
    }
}
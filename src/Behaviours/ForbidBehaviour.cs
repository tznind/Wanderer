using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;

namespace StarshipWanderer.Behaviours
{
    public class ForbidBehaviour<T> : Behaviour<T> where T:IAction
    {
        public ICondition<T> Condition { get; set; }

        public ForbidBehaviour(ICondition<T> condition, IActor owner):base(owner)
        {
            Condition = condition;
        }

        public override void OnPush(IUserinterface ui, ActionStack stack)
        {
            foreach (var matchingAction in stack.Where(a => a is T t && Condition.IsMet(t)).ToArray())
            {
                stack.Push(new ForbidAction(matchingAction,Owner));
                
                //elevate it to cancel pending (allows later actions/behaviours to cancel this action)
                if(matchingAction.Cancelled == CancellationStatus.NotCancelled)
                    matchingAction.Cancelled = CancellationStatus.CancellationPending;
            }
        }
    }
}

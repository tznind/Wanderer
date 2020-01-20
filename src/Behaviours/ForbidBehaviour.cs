using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;

namespace StarshipWanderer.Behaviours
{
    public class ForbidBehaviour<T> : Behaviour where T:Action
    {
        public ICondition<Frame> Condition { get; set; }

        public ForbidBehaviour(ICondition<Frame> condition, IActor owner):base(owner)
        {
            Condition = condition;
        }

        public override void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
            foreach (var matchingFrame in stack.Where(a => a.Action is T && Condition.IsMet(frame)).ToArray())
            {
                var a = new ForbidAction(matchingFrame);
                a.Push(ui,stack,(IActor)Owner);
                
                //elevate it to cancel pending (allows later actions/behaviours to cancel this action)
                if (matchingFrame.Cancelled == CancellationStatus.NotCancelled)
                    matchingFrame.Cancelled = CancellationStatus.CancellationPending;
            }
        }
    }
}

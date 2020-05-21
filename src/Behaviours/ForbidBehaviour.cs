using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Compilation;

namespace Wanderer.Behaviours
{
    public class ForbidBehaviour<T> : Behaviour where T:Action
    {
        public ICondition Condition { get; set; }

        public ForbidBehaviour(ICondition condition, IActor owner):base(owner)
        {
            Condition = condition;
        }

        public override void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            // For testing the condition
            var args = new ActionFrameSystemArgs(frame.PerformedBy,world,ui,stack,frame);

            foreach (var matchingFrame in stack.Where(a => a.Action is T && Condition.IsMet(world,args)).ToArray())
            {
                var a = new ForbidAction(matchingFrame);
                a.Push(world,ui,stack,(IActor)Owner);
            }
        }
    }
}

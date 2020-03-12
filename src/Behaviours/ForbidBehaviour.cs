using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Compilation;

namespace Wanderer.Behaviours
{
    public class ForbidBehaviour<T> : Behaviour where T:Action
    {
        public ICondition<Frame> Condition { get; set; }

        public ForbidBehaviour(ICondition<Frame> condition, IActor owner):base(owner)
        {
            Condition = condition;
        }

        public override void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            foreach (var matchingFrame in stack.Where(a => a.Action is T && Condition.IsMet(world,frame)).ToArray())
            {
                var a = new ForbidAction(matchingFrame);
                a.Push(world,ui,stack,(IActor)Owner);
            }
        }
    }
}

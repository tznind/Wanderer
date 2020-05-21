using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;

namespace Tests.BehaviourTests
{
    class ForbidTests : UnitTest
    {
        [Test]
        public void TestForbidDirection()
        {
            TwoInARoom(out You you, out IActor them, out IWorld w);

            them.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new ConditionCode("return Frame.LeaveDirection == Direction.South"), them));
            var behaviour = them.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();
           
            var stackA = new ActionStack();
            var frameArgsA = new ActionFrameSystemArgs(you,w,null,stackA,new LeaveFrame(them,new LeaveAction(you),Direction.North,0));

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(w,frameArgsA));

            var stackB = new ActionStack();
            var frameArgsB = new ActionFrameSystemArgs(you,w,null,stackB,new LeaveFrame(them,new LeaveAction(you),Direction.South,0));

            //we DO forbid going South
            Assert.IsTrue(behaviour.Condition.IsMet(w,frameArgsB));

            var ui = GetUI(Direction.South);
            w.RunRound(ui,new LeaveAction(you));

            Assert.Contains("Chaos Sam prevented Test Wanderer from performing action Leave",ui.MessagesShown);

        }
    }
}

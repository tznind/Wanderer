using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Compilation;

namespace Tests.BehaviourTests
{
    class ForbidTests : UnitTest
    {
        [Test]
        public void TestForbidDirection()
        {
            TwoInARoom(out _, out IActor them, out IWorld w);

            them.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new ConditionCode<LeaveFrame>("Direction == StarshipWanderer.Direction.South"), them));
            var behaviour = them.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();
            
            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(them,new LeaveAction(),Direction.North,0)));
            //we DO forbid going South
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(them,new LeaveAction(),Direction.South,0)));

            var ui = GetUI(Direction.South);
            w.RunRound(ui,new LeaveAction());

            Assert.Contains("Chaos Sam prevented Test Wanderer from performing action Leave",ui.MessagesShown);

        }
    }
}

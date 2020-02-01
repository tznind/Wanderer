using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;

namespace Tests.BehaviourTests
{
    class ForbidTests : UnitTest
    {
        [Test]
        public void TestForbidDirection()
        {
            TwoInARoom(out _, out IActor them, out IWorld w);

            them.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new LeaveDirectionCondition(Direction.Down), them));
            var behaviour = them.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();
            
            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(them,new LeaveAction(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(them,new LeaveAction(),Direction.Down,0)));

            var ui = GetUI(Direction.South);


        }
    }
}

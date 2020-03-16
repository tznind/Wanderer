using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;

namespace Tests.Actions
{
    class InspectTests : UnitTest
    {
        [Test]
        public void TestInspectIsFreeAction()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var stack = new ActionStack();
            var ui = GetUI(them);

            var action = new InspectAction(you);
            //you can pick them as a target
            Assert.IsTrue(action.HasTargets(you));

            //do so
            Assert.IsEmpty(ui.StatsShown);
            Assert.IsFalse(stack.RunStack(world,ui,action,you,null),"Expected Inspect to be a free action");
            
            //you should have seen their stats
            Assert.Contains(them,ui.StatsShown);
        }
    }
}
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
            TwoInARoom(out You you, out IActor them, out IWorld _);

            var stack = new ActionStack();
            var ui = GetUI(them);

            Assert.IsEmpty(ui.StatsShown);
            Assert.IsFalse(stack.RunStack(Mock.Of<IWorld>(),ui,new InspectAction(),you,null),"Expected Inspect to be a free action");
            Assert.Contains(them,ui.StatsShown);
        }
    }
}
using System;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;

namespace Tests.BehaviourTests
{
    class BehaviourTests
    {
        [Test]
        public void TestBehaviours_AreIdentical()
        {
            var b1 = new ForbidBehaviour<LeaveAction>(new ConditionCode<Frame>("true"), Mock.Of<IActor>());
            var b2 = new ExpiryBehaviour(new Adjective(Mock.Of<IActor>()){Name = "Medic"}, 4);
            var b3 =new ExpiryBehaviour(new Adjective(Mock.Of<IActor>()){Name = "Medic"}, 3);

            Assert.IsFalse(b1.AreIdentical(null));
            Assert.IsFalse(b1.AreIdentical(b2),"Different types");
            Assert.IsFalse(b2.AreIdentical(b1),"Different types");
            
            Assert.IsFalse(b2.AreIdentical(b3),"Different rounds remaining");

            //simulate 1 round
            b2.OnRoundEnding(Mock.Of<IWorld>(),Mock.Of<IUserinterface>(),Guid.NewGuid());

            //now timer matches they are identical from users perspective
            Assert.IsTrue(b2.AreIdentical(b3),"Same rounds remaining now");
        }
    }
}

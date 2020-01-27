using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NLog.LayoutRenderers.Wrappers;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;

namespace Tests
{
    class BehaviourTests
    {
        [Test]
        public void TestBehaviours_AreIdentical()
        {
            var b1 = new ForbidBehaviour<LeaveAction>(new AlwaysCondition<Frame>(), Mock.Of<IActor>());
            var b2 = new ExpiryBehaviour(new Medic(Mock.Of<IActor>()), 4);
            var b3 =new ExpiryBehaviour(new Medic(Mock.Of<IActor>()), 3);

            Assert.IsFalse(b1.AreIdentical(null));
            Assert.IsFalse(b1.AreIdentical(b2),"Different types");
            Assert.IsFalse(b2.AreIdentical(b1),"Different types");
            
            Assert.IsFalse(b2.AreIdentical(b3),"Different rounds remaining");

            //simulate 1 round
            b2.OnRoundEnding(Mock.Of<IUserinterface>(),Guid.NewGuid());

            //now timer matches they are identical from users perspective
            Assert.IsTrue(b2.AreIdentical(b3),"Same rounds remaining now");
        }
    }
}

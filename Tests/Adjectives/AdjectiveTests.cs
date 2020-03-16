using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Stats;

namespace Tests.Adjectives
{
    class AdjectiveTests : UnitTest
    {

        [Test]
        public void Test_AdjectiveEquality()
        {
            var a1 = new Adjective(Mock.Of<IActor>()){Name = "Attractive"};

            var a2 = new Adjective(Mock.Of<IActor>()){Name = "Attractive"};
            Assert.AreNotEqual(a1,a2);

            var ac1 = new AdjectiveCollection {a1};

            var ac2 = new AdjectiveCollection {a2};

            //they are not equal but the user would consider them identical collections
            Assert.IsTrue(ac1.AreIdentical(ac2));
            Assert.AreNotEqual(ac1,ac2);
        }
    }
}

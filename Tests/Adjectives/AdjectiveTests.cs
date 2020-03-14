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
        public void TestAttractive()
        {
            var room = InARoom(out IWorld w);
            var d  = new Npc("Dave",room).With(Stat.Fight,20).With(Stat.Coerce,20);
            
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

            var attractive = new Attractive(d);
            d.Adjectives.Add(attractive);

            Assert.AreEqual(35,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

            var injury = new Injured("Broken Ribs",d,20,InjuryRegion.Head,w.InjurySystems.First(i=>i.IsDefault));
            d.Adjectives.Add(injury);

            //now injured
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(10,d.GetFinalStats()[Stat.Fight]);

            d.Adjectives.Remove(injury);
            
            Assert.AreEqual(35,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

        }

        [Test]
        public void Test_AdjectiveEquality()
        {
            var a1 = new Attractive(Mock.Of<IActor>());

            var a2 = new Attractive(Mock.Of<IActor>());
            Assert.AreNotEqual(a1,a2);

            var ac1 = new AdjectiveCollection {a1};

            var ac2 = new AdjectiveCollection {a2};

            //they are not equal but the user would consider them identical collections
            Assert.IsTrue(ac1.AreIdentical(ac2));
            Assert.AreNotEqual(ac1,ac2);
        }
    }
}

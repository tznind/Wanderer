using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer.Stats;

namespace Tests.Stats
{
    class StatTests
    {
        [Test]
        public void TestStats_AllDescribed()
        {
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
                Assert.IsFalse(string.IsNullOrEmpty(stat.Describe()));
        }

        [Test]
        public void TestStats_AreIdentical()
        {
            var s1 = new StatsCollection();
            var s2 = new StatsCollection();

            Assert.IsFalse(s1.AreIdentical(null));

            Assert.IsTrue(s1.AreIdentical(s1));
            Assert.IsTrue(s1.AreIdentical(s2));

            s2[Stat.Fight] = 10;
            Assert.IsFalse(s1.AreIdentical(s2));

            s1[Stat.Fight] = 10;
            Assert.IsTrue(s1.AreIdentical(s2));
        }   
        
        [Test]
        public void TestStats_IsEmpty()
        {
            var s1 = new StatsCollection();
            
            Assert.IsTrue(s1.IsEmpty());
            s1[Stat.Fight] = 0.0000000000001;
            Assert.IsTrue(s1.IsEmpty());
            s1[Stat.Fight] = 1;
            Assert.IsFalse(s1.IsEmpty());
        }

        
        [Test]
        public void TestStats_Decrease()
        {
            var s1 = new StatsCollection();
            Assert.AreEqual(0,s1[Stat.Fight]);
            s1.Decrease(Stat.Fight,10);
            Assert.AreEqual(-10,s1[Stat.Fight]);
        }
        [Test]
        public void TestStats_Increase()
        {
            var s1 = new StatsCollection();
            Assert.AreEqual(0,s1[Stat.Fight]);
            s1.Increase(Stat.Fight,10);
            Assert.AreEqual(10,s1[Stat.Fight]);
        }
        [Test]
        public void TestStats_IncreaseAll()
        {
            var s1 = new StatsCollection();
            Assert.AreEqual(0,s1[Stat.Fight]);
            s1.Increase(new StatsCollection(10));
            Assert.AreEqual(10,s1[Stat.Fight]);
            Assert.IsTrue(s1.All(v => v.Value == 10));
        }
        [Test]
        public void TestStats_DecreaseAll()
        {
            var s1 = new StatsCollection();
            Assert.AreEqual(0,s1[Stat.Fight]);
            s1.Decrease(new StatsCollection(10));
            Assert.AreEqual(-10,s1[Stat.Fight]);
            Assert.IsTrue(s1.All(v => v.Value == -10));
        }
        [Test]
        public void TestStats_SetAll()
        {
            var s1 = new StatsCollection(5);
            Assert.AreEqual(5,s1[Stat.Fight]);
            Assert.AreEqual(5,s1[Stat.Savvy]);

            s1.SetAll((s,v)=> v * (s == Stat.Fight ? 2 : 1));
            Assert.AreEqual(10,s1[Stat.Fight]);
            Assert.AreEqual(5,s1[Stat.Savvy]);
        }
    }
}

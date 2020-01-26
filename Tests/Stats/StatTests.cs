using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer.Stats;

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
    }
}

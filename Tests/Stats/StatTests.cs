using System;
using System.Collections.Generic;
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
    }
}

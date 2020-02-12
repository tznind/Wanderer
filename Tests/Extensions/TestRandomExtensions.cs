using System;
using NUnit.Framework;
using Wanderer.Extensions;

namespace Tests.Extensions
{
    public class TestRandomExtensions
    {
        [Test]
        public void TestShuffle()
        {
            var strings = new string[0];

            Assert.IsNull(strings.GetRandom(new Random(1)));

            strings = null;
            Assert.IsNull(strings.GetRandom(new Random(1)));

            strings = new[] {"fish"};
            Assert.AreEqual("fish",strings.GetRandom(new Random(1)));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;

namespace Tests
{
    class DirectionTests
    {
        [Test]
        public void TestOpposites()
        {
            Assert.AreEqual(Direction.Down,Direction.Up.Opposite());
            Assert.AreEqual(Direction.Up,Direction.Down.Opposite());
            Assert.AreEqual(Direction.West,Direction.East.Opposite());
            Assert.AreEqual(Direction.North,Direction.South.Opposite());
            Assert.AreEqual(Direction.East,Direction.West.Opposite());
            Assert.AreEqual(Direction.South,Direction.North.Opposite());

            Assert.AreEqual(Direction.None,Direction.None.Opposite());
        }
    }
}

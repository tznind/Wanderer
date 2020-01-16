using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using Tests.Actions;

namespace Tests.Actors
{
    class ActorTests : UnitTest
    {
        [Test]
        public void TestAddOccupant()
        {
            var you = YouInARoom(out IWorld world);
            world.Population.Add(you);

            var room1 = world.Player.CurrentLocation;

            Assert.Contains(you,world.Population.ToArray());

            var frank = new Npc("Frank",room1);

            
            Assert.AreEqual(room1,you.CurrentLocation);
            Assert.AreEqual(room1,frank.CurrentLocation);

            Assert.Contains(you,world.Population.ToArray());
            Assert.Contains(frank,world.Population.ToArray());
        }
    }
}

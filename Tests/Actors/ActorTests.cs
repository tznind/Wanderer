using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace Tests.Actors
{
    class ActorTests
    {
        [Test]
        public void TestAddOccupant()
        {
            var you = new You();
            var world = new World(you, new RoomFactory());

            var room1 = world.CurrentLocation;

            Assert.Contains(you,room1.Occupants.ToArray());

            var frank = new Actor("Frank");
            room1.AddActor(frank);

            Assert.Contains(you,room1.Occupants.ToArray());
            Assert.Contains(frank,room1.Occupants.ToArray());
        }
    }
}

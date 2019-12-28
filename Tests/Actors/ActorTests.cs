using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace Tests.Actors
{
    class ActorTests
    {
        [Test]
        public void TestAddOccupant()
        {
            var you = new You(null);
            var world = new World(you, new RoomFactory(new ActorFactory()));

            var room1 = world.CurrentLocation;

            Assert.Contains(you,room1.Occupants.ToArray());

            var frank = new Actor(world,"Frank");
            room1.AddActor(frank);

            Assert.Contains(you,room1.Occupants.ToArray());
            Assert.Contains(frank,room1.Occupants.ToArray());
        }
    }
}

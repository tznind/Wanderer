using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace Tests.Actors
{
    class ActorTests
    {
        [Test]
        public void TestAddOccupant()
        {

            var world = new World();
            var itemFactory = new ItemFactory();

            var roomFactory = new RoomFactory(new ActorFactory(itemFactory),itemFactory);
            var you = new You("TestPlayer",roomFactory.Create(world));
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

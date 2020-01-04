using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
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
            var adjectiveFactory = new AdjectiveFactory();
            var itemFactory = new ItemFactory(adjectiveFactory);

            var roomFactory = new RoomFactory(new ActorFactory(itemFactory, adjectiveFactory),itemFactory, adjectiveFactory);
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

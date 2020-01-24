using System;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Factories;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Relationships;

namespace Tests.Actors
{
    class YamlRoomFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingRoomFromBlueprint_NoFaction()
        {
            var w = new World();

            var yaml = 
@"
- Name: Tunnels
";
            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());
            var room = roomFactory.Create(w);

            Assert.IsNotNull(room);
            Assert.AreEqual("Tunnels",room.Name);

            Assert.IsEmpty(room.Actors,"Expected that because there are no factions there are no actor factories");
        }

        [Test]
        public void TestCreatingRoomFromBlueprint_WithFaction()
        {
            var w = new World();

            var adj = new AdjectiveFactory();

            w.Factions.Add(
                new Faction("Techno Wizards",FactionRole.Establishment)
                {
                    Identifier = new Guid("bb70f169-e0f7-40e8-927b-1c181eb8740b"),
                    ActorFactory = new ActorFactory(new ItemFactory(adj),adj)
                    {
                        Blueprints = new []
                        {
                            new ActorBlueprint()
                            {
                                Name = "Sandman"
                            }, 
                        }
                    }
                }
            );

            var yaml = 
                @"
- Name: Tunnels
  Faction: bb70f169-e0f7-40e8-927b-1c181eb8740b
";
            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());
            var room = roomFactory.Create(w);

            Assert.IsNotNull(room);
            Assert.AreEqual("Tunnels",room.Name);

            Assert.Greater(room.Actors.Count(),0);
            Assert.IsTrue(room.Actors.All(a=>a.Name.Equals("Sandman")));
        }
    }
}
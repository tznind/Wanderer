using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Factories;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace Tests.Actors
{
    class ActorFactoryTests : UnitTest
    {
        [Test]
        public void TestCreateActors_AppropriateToFaction()
        {
            var adj = new AdjectiveFactory();
            var items = new ItemFactory(adj);

            var actors = new ActorFactory(items, adj);
            
            var world = new World();
            var faction = new Faction("Fish overloards",FactionRole.Wildlife);
            world.Factions.Add(faction);
            faction.ActorFactory = actors;

            actors.Blueprints = new[] {new ActorBlueprint() {Name = "Captain Haddock"}};

            var room = new Room("Tank Bay", world, 't') {ControllingFaction = faction};

            Assert.IsEmpty(room.Actors);
            actors.Create(world, room,faction,null);
            
            Assert.IsTrue(room.Actors.Any());
            Assert.GreaterOrEqual(room.Actors.Count(a=>a.FactionMembership.Contains(faction)),1,"Expected room to be populated with some actors belonging to the controlling faction");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
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

        [Test]
        public void TestActorFactory_SlotCreation()
        {

            string yamlDefaultSlots =
                @"
Head: 1
Hands: 1
Legs: 2
Chest: 1
";

            string yaml = @"
- Name: Pirate
- Name: Scorpion
  Slots:
    Head: 1
    Tail: 1
    Legs: 6
";
            var room = InARoom(out IWorld w);
            var actorFactory = new YamlActorFactory(yaml,yamlDefaultSlots, new ItemFactory(new AdjectiveFactory()), new AdjectiveFactory());
            var scorpion = actorFactory.Create(w, room, null, actorFactory.Blueprints[1],null);

            Assert.AreEqual(1,scorpion.AvailableSlots["Head"]);
            Assert.AreEqual(1,scorpion.AvailableSlots["Tail"]);
            Assert.AreEqual(6,scorpion.AvailableSlots["Legs"]);

            
            var pirate = actorFactory.Create(w, room, null, actorFactory.Blueprints[0],null);
            Assert.AreEqual(1,pirate.AvailableSlots["Head"]);
            Assert.AreEqual(1,pirate.AvailableSlots["Hands"]);
            Assert.AreEqual(2,pirate.AvailableSlots["Legs"]);
            Assert.AreEqual(1,pirate.AvailableSlots["Chest"]);

        }

        
        [Test]
        public void TestActorFactory_ExplicitActions()
        {
            string yaml = @"
- Name: Scorpion
  Slots:
    Head: 1
    Tail: 1
    Legs: 6
  Actions:
    - FightAction()
";
            var room = InARoom(out IWorld w);
            var actorFactory = new YamlActorFactory(yaml,null, new ItemFactory(new AdjectiveFactory()), new AdjectiveFactory());
            var scorpion = actorFactory.Create(w, room, null, actorFactory.Blueprints[0],null);

            Assert.IsInstanceOf<FightAction>(scorpion.BaseActions.Single(),"Expected Scorpion to be capable of nothing but fighting");
        }

    }
}

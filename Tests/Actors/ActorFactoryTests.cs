using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;

namespace Tests.Actors
{
    class ActorFactoryTests : UnitTest
    {
        [Test]
        public void TestCreateActors_AppropriateToFaction()
        {
            var adj = new AdjectiveFactory();
            var items = new ItemFactory();
            var actors = new ActorFactory();
            
            var world = new World();
            var faction = new Faction("Fish overloards",FactionRole.Wildlife);
            world.Factions.Add(faction);
            world.ActorFactory = actors;
            world.ItemFactory = items;

            actors.Blueprints = new List<ActorBlueprint> {new ActorBlueprint() {Name = "Captain Haddock"}};

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



            var actorFactory = new ActorFactory()
            {
                Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(yaml),
                DefaultSlots = Compiler.Instance.Deserializer.Deserialize<SlotCollection>(yamlDefaultSlots),
            };

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
  SkipDefaultActions: true
  Actions:
    - Type: FightAction
";
            var room = InARoom(out IWorld w);

            var actorFactory = new ActorFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(yaml)};
            var scorpion = actorFactory.Create(w, room, null, actorFactory.Blueprints[0],null);

            Assert.IsInstanceOf<FightAction>(scorpion.BaseActions.Single(),"Expected Scorpion to be capable of nothing but fighting");
        }

    }
}

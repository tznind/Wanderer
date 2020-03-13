using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Relationships;

namespace Tests.Actors
{
    class YamlRoomFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingRoomFromBlueprint_WithDialogue()
        {
            var yaml = @"- Name: Gun Bay
  Dialogue:
    Next: 193506ab-11bc-4de2-963e-e2f55a38d006";

            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());

            var w = new World();
            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("193506ab-11bc-4de2-963e-e2f55a38d006"),
                Body = new List<TextBlock>{new TextBlock("This room is rank"), }
            });

            var room = roomFactory.Create(w, roomFactory.Blueprints.Single());
            w.Map.Add(new Point3(0,0,0),room);

            var you = new You("Wanderer",room);

            var ui = GetUI("look:Gun Bay");

            w.RunRound(ui,new DialogueAction());

            Assert.Contains("This room is rank",ui.MessagesShown);


        }

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

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreatingRoomFromBlueprint_WithFaction(bool explicitRoomColor)
        {
            var w = new World();

            var adj = new AdjectiveFactory();

            w.Factions.Add(
                new Faction("Techno Wizards",FactionRole.Establishment)
                {
                    Identifier = new Guid("bb70f169-e0f7-40e8-927b-1c181eb8740b"),
                    Color = ConsoleColor.Cyan,
                }
            );
            w.ActorFactory = new ActorFactory(adj)
            {
                Blueprints = new List<ActorBlueprint>
                {
                    new ActorBlueprint()
                    {
                        Name = "Sandman"
                    },
                }
            };
            w.ItemFactory = new ItemFactory(adj);

            var yaml = 
                @$"
- Name: Tunnels
  {(explicitRoomColor ? "Color: 2" : "")}
  Faction: bb70f169-e0f7-40e8-927b-1c181eb8740b
";
            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());
            var room = roomFactory.Create(w);

            Assert.IsNotNull(room);
            Assert.AreEqual("Tunnels",room.Name);

            Assert.Greater(room.Actors.Count(),0);
            Assert.IsTrue(room.Actors.All(a=>a.Name.Equals("Sandman")));

            //if the room has no set color and it is owned by the faction it should inherit the faction color
            Assert.AreEqual(explicitRoomColor ? ConsoleColor.DarkGreen : ConsoleColor.Cyan ,room.Color);
        }
        
        [Test]
        public void Test_UniqueRooms()
        {
            var g = new Guid("1f0eb057-edac-4eaa-b61b-778b75463cb9");

            var yaml =
                @"
- Identifier: 1f0eb057-edac-4eaa-b61b-778b75463cb9
  Name: BossRoom
  Unique: true
- Name: RegularRoom

";
            var room =  new YamlRoomFactory(yaml, new AdjectiveFactory());

            var w = new World();

            var rooms = new List<IRoom>();
            for (int i = 0; i < 100; i++) 
                rooms.Add(room.Create(w));

            Assert.AreEqual(99,rooms.Count(r=>r.Name.Equals("RegularRoom")));
            Assert.AreEqual(1,rooms.Count(r=>r.Name.Equals("BossRoom")));
        }

        [Test]
        public void SpawnItem_NotFound()
        {
            var you = YouInARoom(out IWorld w);

            Assert.IsEmpty(w.ItemFactory.Blueprints);

            var g = Guid.NewGuid();
            var ex = Assert.Throws<GuidNotFoundException>(()=>you.SpawnItem(g));
            Assert.AreEqual(g,ex.Guid);
        }
        [Test]
        public void SpawnItem_Found()
        {
            var you = YouInARoom(out IWorld w);
            Assert.IsEmpty(you.Items);
            
            var g = Guid.NewGuid();
            w.ItemFactory.Blueprints.Add(new ItemBlueprint()
            {
                Name = "Grenade Pin",
                Identifier = g
            });

            you.SpawnItem(g);

            Assert.AreEqual("Grenade Pin",you.Items.Single().Name);
            Assert.AreEqual(g,you.Items.Single().Identifier);
        }
    }
}
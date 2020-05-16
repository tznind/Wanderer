using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Rooms;
using Wanderer.Stats;

namespace Tests.Relationships
{
    class FactionTests : UnitTest
    {
        [Test]
        public void TestFactionName()
        {
            var faction = new Faction("Cult", FactionRole.Opposition);

            StringAssert.AreEqualIgnoringCase("Cult",faction.Name);
            faction.Adjectives.Add(new Adjective(faction)
            {
                Name = "Dark",
                IsPrefix = true
            });

            StringAssert.AreEqualIgnoringCase("Cult",faction.Name,"Even though the adjective has a prefix we should not be putting it at the front every time this faction is mentioned!");

        }

        [Test]
        public void BeingInFactionGrantsStats()
        {
            var you = YouInARoom(out IWorld _);

            var f = new Faction();
            f.Name = "Geniuses";
            f.BaseStats["Savvy"] = 50;
            
            var before = you.GetFinalStats()["Savvy"];

            you.FactionMembership.Add(f);

            Assert.AreEqual(before + 50, you.GetFinalStats()["Savvy"]);
            you.FactionMembership.Clear();
            
            Assert.AreEqual(before, you.GetFinalStats()["Savvy"]);

        }

        [Test]
        public void BeingInFactionGrantsAdjectives()
        {
            var you = YouInARoom(out IWorld _);
            you.BaseStats["Savvy"] = 50; //allows use of medic skill

            var f = new Faction();
            f.Name = "Medical Corp";
            var medic = new Adjective(f) {Name = "Medic"};
            medic.BaseActions.Add(new HealAction(medic));
            f.Adjectives.Add(medic);
            
            Assert.IsFalse(you.Has("Medic",false));

            you.FactionMembership.Add(f);
            
            Assert.IsTrue(you.Has("Medic",false));
            Assert.AreEqual(1,you.GetFinalActions().OfType<HealAction>().Count());
            
            you.FactionMembership.Clear();

            Assert.IsFalse(you.Has("Medic",false));

        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestRoomFactory_SpawnFactionAppropriateActors(int overload)
        {
            InARoom(out IWorld world);

            world.RoomFactory.Blueprints.Clear();
            world.Factions.Clear();

            world.Factions.Add(new Faction(){
                Name = "Thugs",
                Identifier = new Guid("5571dfab-04e2-458c-8227-a6c5ce446747")
            });

            world.Factions.Add(new Faction(){
                Name = "Gentlemen",
                Identifier = new Guid("003b78ab-9e2a-44c7-888f-ce160c9fe0bf")
            });

            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){
                Name = "Somewhere"
            });

            world.ActorFactory.Blueprints.Add(new ActorBlueprint(){
                Name = "Fisticuff Joe",
                Faction =  new Guid("5571dfab-04e2-458c-8227-a6c5ce446747")
            });

            world.ActorFactory.Blueprints.Add(new ActorBlueprint(){
                Name = "Jac Le Blue",
                Faction =  new Guid("003b78ab-9e2a-44c7-888f-ce160c9fe0bf")
            });


            for(int i = 0 ; i < 10; i++)
            {
                IRoom room = null;
                
                //only used by ActorFactory cases
                var f = world.Factions.GetRandomFaction(world.R);
                
                switch (overload)
                {
                    
                    case 1 : room = world.RoomFactory.Create(world,world.RoomFactory.Blueprints.Single());
                             break;
                    case 2 : room = world.RoomFactory.Create(world);
                             break;
                    case 3 : room = world.RoomFactory.Create(world,location: new Point3(0,1,2));
                             break;
                    case 4 : room = new Room("Somewhere",world,'d'){ControllingFaction = f};
                            world.ActorFactory.Create(world,room,f,world.RoomFactory.Blueprints.Single());
                            break;
                    case 5 : room = new Room("Somewhere",world,'d'){ControllingFaction = f};
                            world.ActorFactory.Create(world,room,f,world.ActorFactory.Blueprints.Single(b=>b.SuitsFaction(f)),world.RoomFactory.Blueprints.Single());
                            break;
                }

                Assert.IsNotNull(room.ControllingFaction);
                Assert.IsNotEmpty(room.Actors);

                if(room.ControllingFaction.Identifier == new Guid("003b78ab-9e2a-44c7-888f-ce160c9fe0bf"))
                    Assert.True(room.Actors.All(a=>a.Name.Equals("Jac Le Blue")),"Expected only Faction appropriate members to appear in faction aligned rooms");
                else
                    Assert.True(room.Actors.All(a=>a.Name.Equals("Fisticuff Joe")),"Expected only Faction appropriate members to appear in faction aligned rooms");
            }
        }
        
        [Test]
        public void TestUnaligned_OnRoom()
        {
            InARoom(out IWorld world);
            
            world.ActorFactory.Blueprints.Add(new ActorBlueprint(){Name = "Hedgehog"});


            Assert.IsEmpty(world.Factions);
            var room = world.RoomFactory.Create(world, new RoomBlueprint() {Name = "Dangerous Pit"});
            Assert.IsNull(room.ControllingFaction,"When there are no factions in the world rooms have no faction");
            Assert.IsFalse(room.Actors.First().FactionMembership.Any());

            world.Factions.Add(new Faction("Cultists",FactionRole.Opposition));
            var room2 = world.RoomFactory.Create(world, new RoomBlueprint() {Name = "Dangerous Pit"});

            Assert.AreEqual(world.Factions.Single(),room2.ControllingFaction,"When there are factions in the world, rooms get assigned to random factions");
            Assert.IsTrue(room2.Actors.First().FactionMembership.Contains(world.Factions.Single()));

            var room3 = world.RoomFactory.Create(world, new RoomBlueprint()
            {
                Name = "Dangerous Pit",
                Unaligned = true
            });

            Assert.IsNull(room3.ControllingFaction,"Unaligned rooms should not have a faction");
            Assert.IsFalse(room3.Actors.First().FactionMembership.Any());
        }
        [TestCase(true)]
        [TestCase(false)]
        public void TestUnaligned_OnActor(bool unaligned)
        {
            InARoom(out IWorld world);
            var f = new Faction("Cultists", FactionRole.Opposition)
            {
                Identifier  = new Guid("25d4a44c-ea41-48a6-aa0a-2bfcac2cf173")
            };

            world.ActorFactory.Blueprints.Add(new ActorBlueprint()
            {
                Name = "Hedgehog",
                Unaligned = unaligned,
                Faction = f.Identifier
            });

            world.Factions.Add(f);
            
            var room = world.RoomFactory.Create(world, new RoomBlueprint()
            {
                Name = "Dangerous Pit"
            });

            //room should have faction but actor is unaligned so shouldn't
            Assert.AreEqual(world.Factions.Single(),room.ControllingFaction);

            if(unaligned)
                Assert.IsEmpty(room.Actors.First().FactionMembership.ToArray());
            else
                Assert.AreEqual(world.Factions.Single(),room.Actors.First().FactionMembership.Single());
        }
    }
}

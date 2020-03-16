﻿using System;
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
        public void BeingInFactionGrantsStats()
        {
            var you = YouInARoom(out IWorld _);

            var f = new Faction();
            f.Name = "Geniuses";
            f.BaseStats[Stat.Savvy] = 50;
            
            var before = you.GetFinalStats()[Stat.Savvy];

            you.FactionMembership.Add(f);

            Assert.AreEqual(before + 50, you.GetFinalStats()[Stat.Savvy]);
            you.FactionMembership.Clear();
            
            Assert.AreEqual(before, you.GetFinalStats()[Stat.Savvy]);

        }

        [Test]
        public void BeingInFactionGrantsAdjectives()
        {
            var you = YouInARoom(out IWorld _);
            you.BaseStats[Stat.Savvy] = 50; //allows use of medic skill

            var f = new Faction();
            f.Name = "Medical Corp";
            f.Adjectives.Add(new Adjective(f){Name = "Medic", BaseActions = {new HealAction()}});
            
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
                
                switch (overload)
                {
                    
                    case 1 : room = world.RoomFactory.Create(world,world.RoomFactory.Blueprints.Single());
                             break;
                    case 2 : room = world.RoomFactory.Create(world);
                             break;
                    case 3 : room = world.RoomFactory.Create(world,location: new Point3(0,1,2));
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
    }
}

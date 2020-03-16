using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests.Systems
{
    public class SpreadingTests : UnitTest
    {
        [TestCase(true,true)]
        [TestCase(false,true)]
        [TestCase(true,false)]
        [TestCase(false,false)]
        public void TestSpreading_FromRoom(bool toActors, bool toRooms)
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var otherRoom = world.RoomFactory.Create(world);
            world.Map.Add(new Point3(1, 0, 0),otherRoom);
            
            var system = new InjurySystem()
            {
                Identifier = Guid.NewGuid(),
                Name = "Corruption",
                Spreads = new Spreading()
                {
                    RoomsToActors = toActors,
                    RoomsToRooms = toRooms
                },
                Injuries = new List<InjuryBlueprint>(){new InjuryBlueprint("Corrupted",10)}
            };

            system.Apply(new SystemArgs(world,GetUI(),10,null,you.CurrentLocation,Guid.Empty));

            //the room must have it
            Assert.IsTrue(you.CurrentLocation.Has(system.Identifier));
            Assert.IsFalse(you.Has(system.Identifier));
            Assert.IsFalse(them.Has(system.Identifier));
            Assert.IsFalse(otherRoom.Has(system.Identifier));

            //make the injury worse (also triggers spreading)
            system.Worsen((Injured) you.CurrentLocation.Get(system.Identifier).Single(),GetUI(),Guid.Empty);

            //room should still have it
            Assert.AreEqual(true,you.CurrentLocation.Has(system.Identifier));

            //it should have spread accordingly
            Assert.AreEqual(toRooms,otherRoom.Has(system.Identifier));
            Assert.AreEqual(toActors,you.Has(system.Identifier));
            Assert.AreEqual(toActors,them.Has(system.Identifier));

        }


        [TestCase(true,true)]
        [TestCase(false,true)]
        [TestCase(true,false)]
        [TestCase(false,false)]
        public void TestSpreading_FromActor(bool toActors, bool toRooms)
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var otherRoom = world.RoomFactory.Create(world);
            world.Map.Add(new Point3(1, 0, 0),otherRoom);
            
            var system = new InjurySystem()
            {
                Identifier = Guid.NewGuid(),
                Name = "Corruption",
                Spreads = new Spreading()
                {
                    ActorsToActors = toActors,
                    ActorsToRooms = toRooms
                },
                Injuries = new List<InjuryBlueprint>(){new InjuryBlueprint("Corrupted",10)}
            };

            system.Apply(new SystemArgs(world,GetUI(),10,null,you,Guid.Empty));

            //you must have it
            Assert.IsTrue(you.Has(system.Identifier));

            //but nobody/nothing else
            Assert.IsEmpty(you.CurrentLocation.Adjectives);
            Assert.IsFalse(them.Has(system.Identifier));
            Assert.IsEmpty(you.CurrentLocation.Adjectives);

            //make the injury worse (also triggers spreading)
            system.Worsen((Injured) you.Get(system.Identifier).Single(),GetUI(),Guid.Empty);

            //you should still have it
            Assert.AreEqual(true,you.Has(system.Identifier));

            //it should have spread accordingly
            Assert.AreEqual(toRooms,you.CurrentLocation.Adjectives.Any(a=>a.Has(system.Identifier)));
            Assert.AreEqual(toActors,them.Has(system.Identifier));

            //it should never spread from an Actor to an adjacent room!
            Assert.IsFalse(otherRoom.Has(system.Identifier));

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Places;

namespace Tests
{
    class WorldTests
    {
        [Test]
        public void Test_Serialization_OfWorld()
        {
            var config =  World.GetJsonSerializerSettings();

            var world1 = new World(new You(), new RoomFactory(new ActorFactory()));

            var omg = new Actor("omgz");
            omg.AddBehaviour(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new Leave(world1,omg){Direction = Direction.North}));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new Leave(world1,omg){Direction = Direction.Down}));
            
            world1.CurrentLocation.AddActor(omg);

            var actionsBefore = world1.CurrentLocation.GetActions().Count;

            var json = JsonConvert.SerializeObject(world1,config);

            var world2 = (World) JsonConvert.DeserializeObject(json,typeof(World),config);

            Assert.AreEqual(
                world1.CurrentLocation.Title,
                world2.CurrentLocation.Title);

            
            Assert.AreEqual(
                world1.CurrentLocation.Occupants.Count,
                world2.CurrentLocation.Occupants.Count);

            var omg2 = world2.CurrentLocation.Occupants.Single(o => o.Name.Equals("omgz"));

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new Leave(world2,omg2){Direction = Direction.North}));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new Leave(world2,omg2){Direction = Direction.Down}));
            
            Assert.AreEqual(actionsBefore , world2.CurrentLocation.GetActions().Count);
        }

        [Test]
        public void Test_Serialization_OfActor()
        {
            var world1 = new World(new You(), new RoomFactory(new ActorFactory()));

            var omg = new Actor("omgz");
            omg.AddBehaviour(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new Leave(world1,omg){Direction = Direction.North}));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new Leave(world1,omg){Direction = Direction.Down}));
            
            world1.CurrentLocation.AddActor(omg);

            var config = World.GetJsonSerializerSettings();

            var json = JsonConvert.SerializeObject(omg,config);

            var omg2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new Leave(world1,omg2){Direction = Direction.North}));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new Leave(world1,omg2){Direction = Direction.Down}));

        }
    }
}

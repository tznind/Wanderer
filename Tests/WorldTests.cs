using System;
using System.Collections.Generic;
using System.IO;
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
    class WorldTests: UnitTest
    {
        [Test]
        public void Test_Serialization_OfWorld()
        {
            var config =  World.GetJsonSerializerSettings();
            
            var world1 = new WorldFactory()
            {
                ResourcesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,"Resources")
            }.Create();
            
            var omg = new Npc("omgz",world1.Player.CurrentLocation);
            omg.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();



            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down,0)));
            
            var actionsBefore = world1.Player.CurrentLocation.GetFinalActions(world1.Player).Count();

            var json = JsonConvert.SerializeObject(world1,config);

            var world2 = (World) JsonConvert.DeserializeObject(json,typeof(World),config);

            Assert.IsNotNull(world1.Player.CurrentLocation);
                
            Assert.IsNotNull(world2.Player.CurrentLocation);

            Assert.AreEqual(
                world1.Player.CurrentLocation.Name,
                world2.Player.CurrentLocation.Name);
            
            Assert.AreEqual(
                world1.Population.Count,
                world2.Population.Count);

            var omg2 = world2.Population.Single(o => o.Name.Equals("omgz"));

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new LeaveFrame(omg2,new Leave(),Direction.Down,0)));
            
            Assert.AreEqual(actionsBefore , world2.Player.CurrentLocation.GetFinalActions(world2.Player).Count());
        }

        [Test]
        public void Test_Serialization_OfActor()
        {
            var world1 = new WorldFactory()
            {
                ResourcesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,"Resources")
            }.Create();

            var omg = new Npc("omgz",world1.Player.CurrentLocation);
            omg.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down,0)));
            
            var config = World.GetJsonSerializerSettings();

            var json = JsonConvert.SerializeObject(omg,config);

            var omg2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down,0)));

        }
    }
}

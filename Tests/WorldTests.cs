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

            var world1 = new WorldFactory().Create();
            
            var omg = new Npc("omgz",world1.Player.CurrentLocation);
            omg.AddBehaviour(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();



            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down)));
            
            var actionsBefore = world1.Player.CurrentLocation.GetActions(world1.Player).Count;

            var json = JsonConvert.SerializeObject(world1,config);

            var world2 = (World) JsonConvert.DeserializeObject(json,typeof(World),config);

            Assert.IsNotNull(world1.Player.CurrentLocation);
                
            Assert.IsNotNull(world2.Player.CurrentLocation);

            Assert.AreEqual(
                world1.Player.CurrentLocation.Title,
                world2.Player.CurrentLocation.Title);
            
            Assert.AreEqual(
                world1.Population.Count,
                world2.Population.Count);

            var omg2 = world2.Population.Single(o => o.Name.Equals("omgz"));

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North)));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new LeaveFrame(omg2,new Leave(),Direction.Down)));
            
            Assert.AreEqual(actionsBefore , world2.Player.CurrentLocation.GetActions(world2.Player).Count);
        }

        [Test]
        public void Test_Serialization_OfActor()
        {
            var world1 = new WorldFactory().Create();

            var omg = new Npc("omgz",world1.Player.CurrentLocation);
            omg.AddBehaviour(new ForbidBehaviour<Leave>(new LeaveDirectionCondition(Direction.Down), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down)));
            
            var config = World.GetJsonSerializerSettings();

            var json = JsonConvert.SerializeObject(omg,config);

            var omg2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<Leave>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.North)));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new LeaveFrame(omg,new Leave(),Direction.Down)));

        }
    }
}

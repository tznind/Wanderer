using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Places;

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

            var faction = world1.Factions.First();
            var actorFactory = faction.ActorFactory;
            var npc = 
                actorFactory.Create(world1,world1.Player.CurrentLocation,faction,
                    new ActorBlueprint()
                    {
                        Name = "omg",
                        MandatoryAdjectives = new []{new AdjectiveBlueprint(){Type = "Rusty"}}
                    },null);

            Assert.AreEqual(1,npc.Adjectives.OfType<Rusty>().Count(),"Expected npc to be Rusty");
            
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

            var omg2 = world2.Population.Single(o => o.Name.Equals("omg"));
            
            Assert.AreEqual(omg2,omg2.Adjectives.OfType<Rusty>().Single().Owner);
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
            omg.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new ConditionCode<LeaveFrame>("Direction == Wanderer.Direction.Down"), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(new LeaveFrame(omg,new LeaveAction(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(new LeaveFrame(omg,new LeaveAction(),Direction.Down,0)));
            
            var config = World.GetJsonSerializerSettings();

            var json = JsonConvert.SerializeObject(omg,config);

            var omg2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(new LeaveFrame(omg,new LeaveAction(),Direction.North,0)));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(new LeaveFrame(omg,new LeaveAction(),Direction.Down,0)));

        }
    }
}

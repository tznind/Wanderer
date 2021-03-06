﻿using System;
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
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Systems;

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

            var actorFactory = world1.ActorFactory;
            var npc = 
                actorFactory.Create(world1,world1.Player.CurrentLocation,null,
                    new ActorBlueprint()
                    {
                        Name = "omg",
                        MandatoryAdjectives = new []{"352958e3-c210-44db-8ed6-44f0005e0f26"}
                    },null);

            Assert.AreEqual(1,npc.Adjectives.Count(a => a.Name.Equals("Rusty")),"Expected npc to be Rusty");
            
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
            
            Assert.AreEqual(omg2,omg2.Adjectives.Single(a => a.Name.Equals("Rusty")).Owner);
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
            omg.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new ConditionCode("return Frame.LeaveDirection == Direction.Down"), omg));
            
            var behaviour = omg.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour.Condition.IsMet(world1,GetArgs(world1.Player,world1,new LeaveFrame(omg,new LeaveAction(omg),Direction.North,0))));
            //we DO forbid going down
            Assert.IsTrue(behaviour.Condition.IsMet(world1,GetArgs(world1.Player,world1,new LeaveFrame(omg,new LeaveAction(omg),Direction.Down,0))));
            
            var config = World.GetJsonSerializerSettings();

            var json = JsonConvert.SerializeObject(omg,config);

            var omg2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            var behaviour2 = omg2.GetFinalBehaviours().OfType<ForbidBehaviour<LeaveAction>>().Single();

            //we don't forbid going north
            Assert.IsFalse(behaviour2.Condition.IsMet(world1,GetArgs(world1.Player,world1,new LeaveFrame(omg,new LeaveAction(omg),Direction.North,0))));
            //we DO forbid going down
            Assert.IsTrue(behaviour2.Condition.IsMet(world1,GetArgs(world1.Player,world1,new LeaveFrame(omg,new LeaveAction(omg),Direction.Down,0))));

        }

        private SystemArgs GetArgs(IActor a, IWorld w,LeaveFrame leaveFrame)
        {
            var stackA = new ActionStack();
            return new ActionFrameSystemArgs(a,w,null,stackA,leaveFrame);
        }

        [Test]
        public void Test_Serialization_OfExpire()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            them.Adjectives.Add(new Adjective(them)
            {
                Name = "happy"
            }.WithExpiry(10));
            
            
            var config = World.GetJsonSerializerSettings();
            var json = JsonConvert.SerializeObject(world,config);

            var world2 = (World) JsonConvert.DeserializeObject(json,typeof(World),config);

            var them2 = world2.Population.OfType<Npc>().Single();

            Assert.IsNotNull(them.GetFinalBehaviours().OfType<ExpiryBehaviour>().Single().Owner);
            Assert.IsNotNull(them2.GetFinalBehaviours().OfType<ExpiryBehaviour>().Single().Owner);

            Assert.IsNotNull(them.GetFinalBehaviours().OfType<ExpiryBehaviour>().Single().Adjective);
            Assert.IsNotNull(them2.GetFinalBehaviours().OfType<ExpiryBehaviour>().Single().Adjective);
        }

        [Test]
        public void Test_Serialization_OfSingleUse()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var grenade1 = new Item("Grenade");
            grenade1.Adjectives.Add(new SingleUse(grenade1));

            them.Items.Add(grenade1);
            
            var config = World.GetJsonSerializerSettings();
            var json = JsonConvert.SerializeObject(world,config);

            var world2 = (IWorld) JsonConvert.DeserializeObject(json,typeof(World),config);

            var grenade2 = world2.Population.OfType<Npc>().Single().Items.Single();

            Assert.IsNotNull(grenade1.Adjectives.Single().Owner);
            Assert.IsNotNull(grenade2.Adjectives.Single().Owner);
            
            Assert.IsNotNull(grenade1.Adjectives.Single().BaseBehaviours.Single().Owner);
            Assert.IsNotNull(grenade2.Adjectives.Single().BaseBehaviours.Single().Owner);
        }
        [Test]
        public void Test_Serialization_OfActorAdjectives()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);
            them.Adjectives.Add(new Adjective(them){Name = "Tough"});

            var config = World.GetJsonSerializerSettings();
            var json = JsonConvert.SerializeObject(them,config);

            var them2 = (Actor) JsonConvert.DeserializeObject(json,typeof(Actor),config);

            Assert.IsNotNull(them.Adjectives.Single().Owner);
            Assert.IsNotNull(them2.Adjectives.Single().Owner);
        }

        [Test]
        public void Test_Serialization_OfItemAdjectives()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var hammer1 = new Item("Hammer");
            hammer1.Adjectives.Add(new Adjective(hammer1){Name = "Tough"});

            them.Items.Add(hammer1);
            
            var config = World.GetJsonSerializerSettings();
            var json = JsonConvert.SerializeObject(world,config);

            var world2 = (IWorld) JsonConvert.DeserializeObject(json,typeof(World),config);

            var hammer2 = world2.Population.OfType<Npc>().Single().Items.Single();

            Assert.IsNotNull(hammer1.Adjectives.Single().Owner);
            Assert.IsNotNull(hammer2.Adjectives.Single().Owner);

        }

        [Test]
        public void Test_Serialization_OfDynamicDictionary()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            them.V["Fish"] = 10;
            
            var config = World.GetJsonSerializerSettings();
            var json = JsonConvert.SerializeObject(them,config);

            var them2 = (Npc) JsonConvert.DeserializeObject(json,typeof(Npc),config);

            Assert.AreEqual(10,them.V["Fish"]);
            Assert.AreEqual(10,them2.V["Fish"]);

        }

        [Test]
        public void Test_GetSystem()
        {
            var world = new World();
            
            Assert.Throws<NamedObjectNotFoundException>(()=>world.GetSystem("fff"));
            Assert.Throws<GuidNotFoundException>(
                () => world.GetSystem(new Guid("1f0c7480-d434-48ec-9872-aed16c47f2eb")));

            world.InjurySystems.Add(new InjurySystem()
            {
                Name = "fff",
                Identifier = new Guid("1f0c7480-d434-48ec-9872-aed16c47f2eb")
            });

            
            Assert.AreEqual(world.InjurySystems.Single(),world.GetSystem("fff"));
            Assert.AreEqual(world.InjurySystems.Single(),world.GetSystem(new Guid("1f0c7480-d434-48ec-9872-aed16c47f2eb")));

        }
    }
}

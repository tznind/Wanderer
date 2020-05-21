using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Tests.Actions
{
    class LeaveTests : UnitTest
    {
        [Test]
        public void TestLeave_HasTargets()
        {
            var you =YouInARoom(out _);

            Assert.IsTrue(new LeaveAction(you).HasTargets(you));

            you.CurrentLocation.LeaveDirections.Clear();

            Assert.IsFalse(new LeaveAction(you).HasTargets(you));

        }

        [Test]
        public void LeaveAndComeBack()
        {
            var you = YouInARoom(out IWorld world);
            world.RoomFactory = new RoomFactory();
            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){
                Name = "North Room",
                Tile = '-',
                FixedLocation = new Point3(0,1,0)
            });
            
            Assert.AreEqual(new Point3(0,0,0),world.Map.GetPoint(world.Player.CurrentLocation));
            
            var leave = new LeaveAction(you);

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.AreEqual("TestRoom",room1.Name);

            var stack = new ActionStack();
            stack.RunStack(world,GetUI(Direction.North),leave,world.Player,new IBehaviour[0]);
            
            var room2 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");
            Assert.AreEqual("North Room",world.Player.CurrentLocation.Name);
            
            stack.RunStack(world,GetUI(Direction.South),leave,world.Player,new IBehaviour[0]);

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
            Assert.AreEqual("TestRoom",world.Player.CurrentLocation.Name);
        }

        [Test]
        public void WalkInCircle()
        {
            YouInARoom(out IWorld world);
            world.RoomFactory = new RoomFactory();
            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){Name = "NorthRoom", Tile = '-',
                FixedLocation = new Point3(0,1,0)});
            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){Name = "NorthEastRoom", Tile = '-',
                FixedLocation = new Point3(1,1,0)});
            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){Name = "EastRoom", Tile = '-',
                FixedLocation = new Point3(1,0,0)});

            var leave = new LeaveAction(world.Player);

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            //north
            stack.RunStack(world,GetUI(Direction.North),leave,world.Player,new IBehaviour[0]);
            Assert.AreNotSame(room1, world.Player.CurrentLocation,"After going north we should not be in the same room");
            Assert.AreEqual("NorthRoom", world.Player.CurrentLocation.Name);

            //east,south,west
            stack.RunStack(world,GetUI(Direction.East),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("NorthEastRoom", world.Player.CurrentLocation.Name);

            stack.RunStack(world,GetUI(Direction.South),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("EastRoom", world.Player.CurrentLocation.Name);

            stack.RunStack(world,GetUI(Direction.West),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("TestRoom", world.Player.CurrentLocation.Name);
            
            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        /// <summary>
        /// If you go up from a room which allows going up (e.g. stairs) then you should be in a room
        /// that allows down even if that room normally wouldn't allow it
        /// </summary>
        [Test]
        public void WalkUpThenDownAgain()
        {
            var you = YouInARoom(out IWorld world);
            world.RoomFactory = new RoomFactory();

            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){Name = "Start", Tile = '-',
                FixedLocation = new Point3(0,0,0),
                
                //let the player go up from here (normally only horizontal movement is allowed)
                LeaveDirections = new []{Direction.Up}
            });
            world.RoomFactory.Blueprints.Add(new RoomBlueprint(){Name = "UpperRoom", Tile = '-',
                FixedLocation = new Point3(0,0,1)
            });
                        
            var leave = new LeaveAction(you);
            
            var stack = new ActionStack();
            world.Map[new Point3(0, 0, 0)] = world.GetNewRoom(new Point3(0, 0, 0));
            world.Player.CurrentLocation = world.Map[new Point3(0, 0, 0)];
            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            //should start in the starting room
            Assert.AreEqual("Start", world.Player.CurrentLocation.Name);

            //up
            stack.RunStack(world,GetUI(Direction.Up),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("UpperRoom", world.Player.CurrentLocation.Name);
            
            //down
            stack.RunStack(world,GetUI(Direction.Down),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("Start", world.Player.CurrentLocation.Name);

            
            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var room = YouInARoom(out IWorld world).CurrentLocation;
            var guard = new Npc("Guard",room);
            guard.BaseBehaviours.Add(new ForbidBehaviour<LeaveAction>(new ConditionCode("return true"),guard));
            
            var leave = new LeaveAction(world.Player);

            var stack = new ActionStack();

            stack.RunStack(world,GetUI(Direction.South), leave,world.Player ,guard.GetFinalBehaviours());

            Assert.AreSame(room,world.Player.CurrentLocation,"Expected to be in the same room as started in");
        }

        [Test]
        public void FixedLocationRooms()
        {
            var start = new RoomBlueprint()
            {
                Name = "Start",
                FixedLocation = new Point3(0, 0, 0)
            };
            var west = new RoomBlueprint()
            {
                Name = "West",
                FixedLocation = new Point3(-1, 0, 0)
            };

            var f = new RoomFactory()
            {
                Blueprints = new List<RoomBlueprint> {start, west}
            };
            
            var world = new World();
            world.RoomFactory = f;
            var room = world.RoomFactory.Create(world, new Point3(0, 0, 0));
            world.Map.Add(new Point3(0,0,0),room); 
            new You("Test Wanderer", room);

            Assert.AreEqual("Start",world.Player.CurrentLocation.Name);

            //go north
            var stack = new ActionStack();
            stack.RunStack(world,GetUI(Direction.North),new LeaveAction(world.Player), world.Player,new IBehaviour[0]);
            Assert.AreEqual("Empty Room",world.Player.CurrentLocation.Name);

            //go west 
            stack.RunStack(world,GetUI(Direction.West),new LeaveAction(world.Player), world.Player,new IBehaviour[0]);
            Assert.AreEqual("Empty Room",world.Player.CurrentLocation.Name);

            //go south 
            stack.RunStack(world,GetUI(Direction.South),new LeaveAction(world.Player), world.Player,new IBehaviour[0]);
            Assert.AreEqual("West",world.Player.CurrentLocation.Name);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestReveal(bool alreadyExisted)
        {
            var start = new RoomBlueprint()
            {
                Name = "Start",
                FixedLocation = new Point3(0, 0, 0)
            };
            var west = new RoomBlueprint()
            {
                Name = "West",
                FixedLocation = new Point3(-1, 0, 0)
            };

            var f = new RoomFactory()
            {
                Blueprints = new List<RoomBlueprint> {start, west}
            };
            
            var world = new World();
            world.RoomFactory = f;
            var room = world.RoomFactory.Create(world, new Point3(0, 0, 0));
            world.Map.Add(new Point3(0,0,0),room);

            if (alreadyExisted)
            {
                world.Map.Add(new Point3(-1,0,0),world.GetNewRoom(new Point3(-1,0,0))); 
                Assert.IsFalse(world.Map[new Point3(-1,0,0)].IsExplored);
            }
            else
            {
                Assert.IsFalse(world.Map.ContainsKey(new Point3(-1,0,0)));
            }

            world.Reveal(new Point3(-1, 0, 0));
            Assert.IsTrue(world.Map[new Point3(-1,0,0)].IsExplored);
        }
    }
}

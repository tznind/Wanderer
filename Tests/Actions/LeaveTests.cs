using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Factories;
using StarshipWanderer.Items;

namespace Tests.Actions
{
    class LeaveTests : UnitTest
    {
        [Test]
        public void LeaveAndComeBack()
        {
            YouInARoom(out IWorld world);
            var factory = new Mock<IRoomFactory>();

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room("North Room",world,'-'))
                .Throws<Exception>();
            
            world.RoomFactory = factory.Object;
            
            Assert.AreEqual(new Point3(0,0,0),world.Map.GetPoint(world.Player.CurrentLocation));
            
            var leave = new Leave();

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.AreEqual("TestRoom",room1.Name);

            var stack = new ActionStack();
            stack.RunStack(GetUI(Direction.North),leave,world.Player,new IBehaviour[0]);
            
            var room2 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");
            Assert.AreEqual("North Room",world.Player.CurrentLocation.Name);
            
            stack.RunStack(GetUI(Direction.South),leave,world.Player,new IBehaviour[0]);

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
            Assert.AreEqual("TestRoom",world.Player.CurrentLocation.Name);
        }

        [Test]
        public void WalkInCircle()
        {

            var factory = new Mock<IRoomFactory>();
            YouInARoom(out IWorld world);

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room("NorthRoom",world,'-')) 
                .Returns(new Room("NorthEastRoom",world,'-'))
                .Returns(new Room( "EastRoom",world,'-'));
            //next return will be null and should not be invoked.  After all going West
            //from the EastRoom should result in being back in StartingRoom
            
            world.RoomFactory = factory.Object;

            var leave = new Leave();

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            //north
            stack.RunStack(GetUI(Direction.North),leave,world.Player,new IBehaviour[0]);
            Assert.AreNotSame(room1, world.Player.CurrentLocation,"After going north we should not be in the same room");
            Assert.AreEqual("NorthRoom", world.Player.CurrentLocation.Name);

            //east,south,west
            stack.RunStack(GetUI(Direction.East),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("NorthEastRoom", world.Player.CurrentLocation.Name);

            stack.RunStack(GetUI(Direction.South),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("EastRoom", world.Player.CurrentLocation.Name);

            stack.RunStack(GetUI(Direction.West),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("TestRoom", world.Player.CurrentLocation.Name);

            factory.Verify();

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        /// <summary>
        /// If you go up from a room which allows going up (e.g. stairs) then you should be in a room
        /// that allows down even if that room normally wouldn't allow it
        /// </summary>
        [Test]
        public void WalkUpThenDownAgain()
        {
            var factory = new Mock<IRoomFactory>();
            YouInARoom(out IWorld world);

            //notice that this room normally doesn't allow going down
            var upperRoom = new Room("UpperRoom", world, '-');
            Assert.IsFalse(upperRoom.LeaveDirections.Contains(Direction.Down));
            
            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(upperRoom);
            
            world.RoomFactory = factory.Object;
            //let the player go up from here
            world.Player.CurrentLocation.LeaveDirections.Add(Direction.Up);

            var leave = new Leave();

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            Assert.AreEqual("TestRoom", world.Player.CurrentLocation.Name);

            //up
            stack.RunStack(GetUI(Direction.Up),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("UpperRoom", world.Player.CurrentLocation.Name);
            
            Assert.Contains(Direction.Down,upperRoom.LeaveDirections.ToArray());

            //down
            stack.RunStack(GetUI(Direction.Down),leave,world.Player,new IBehaviour[0]);
            Assert.AreEqual("TestRoom", world.Player.CurrentLocation.Name);

            
            factory.Verify();

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var room = YouInARoom(out IWorld world).CurrentLocation;
            var guard = new Npc("Guard",room);
            guard.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new AlwaysCondition<Frame>(),guard));
            
            var leave = new Leave();

            var stack = new ActionStack();

            stack.RunStack(GetUI(Direction.South), leave,world.Player ,guard.GetFinalBehaviours());

            Assert.AreSame(room,world.Player.CurrentLocation,"Expected to be in the same room as started in");
        }
    }
}

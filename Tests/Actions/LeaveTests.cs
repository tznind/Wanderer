using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Places;
using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;

namespace Tests.Actions
{
    class LeaveTests
    {
        [Test]
        public void LeaveAndComeBack()
        {

            var factory = new Mock<IRoomFactory>();
            var world = new World();

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room{Title = "Starting Room"})
                .Returns(new Room{Title = "North Room"})
                .Throws<Exception>();
            
            world.Player = new You();
            world.RoomFactory = factory.Object;
            world.CurrentLocation = factory.Object.Create(world);
            world.Map.Add(new Point3(0,0,0), world.CurrentLocation);
            
            Assert.AreEqual(new Point3(0,0,0),world.Map.GetPoint(world.CurrentLocation));

            var leave = new Leave(world,world.Player);
            var uiGoSouth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.South);
            var uiGoNorth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.North);

            var room1 = world.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();
            stack.RunStack(uiGoNorth,leave,new IBehaviour[0]);
            
            var room2 = world.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");
            
            stack.RunStack(uiGoSouth,leave,new IBehaviour[0]);

            Assert.AreSame(room1, world.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void WalkInCircle()
        {

            var factory = new Mock<IRoomFactory>();
            var world = new World();

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room {Title = "StartingRoom"})
                .Returns(new Room{Title = "NorthRoom"}) 
                .Returns(new Room{Title = "NorthEastRoom"})
                .Returns(new Room{Title = "EastRoom"});
            //next return will be null and should not be invoked.  After all going West
            //from the EastRoom should result in being back in StartingRoom
            
            var ui = new Mock<IUserinterface>();
            ui.SetupSequence(u => u.GetOption<Direction>(It.IsAny<string>()))
                .Returns(Direction.North)
                .Returns(Direction.East)
                .Returns(Direction.South)
                .Returns(Direction.West);

            world.Player = new You();
            world.RoomFactory = factory.Object;
            world.CurrentLocation = factory.Object.Create(world);
            world.Map.Add(new Point3(0,0,0), world.CurrentLocation);

            var leave = new Leave(world,world.Player);

            var room1 = world.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            //north
            stack.RunStack(ui.Object,leave,new IBehaviour[0]);
            Assert.AreNotSame(room1, world.CurrentLocation,"After going north we should not be in the same room");

            //east,south,west
            stack.RunStack(ui.Object,leave,new IBehaviour[0]);
            stack.RunStack(ui.Object,leave,new IBehaviour[0]);
            stack.RunStack(ui.Object,leave,new IBehaviour[0]);

            ui.Verify();
            factory.Verify();

            Assert.AreSame(room1, world.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var world = new World
            {
                Player = new You()
            };

            var room1 = world.CurrentLocation = new Room {Title = "Hotel California"};

            var guard = new Actor("Guard");

            guard.AddBehaviour(new ForbidBehaviour<Leave>(new AlwaysCondition<IAction>(),guard));

            world.CurrentLocation = room1;

            var leave = new Leave(world,world.Player);
            var ui = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.South);

            var stack = new ActionStack();

            stack.RunStack(ui, leave, guard.GetFinalBehaviours());

            Assert.AreSame(room1,world.CurrentLocation,"Expected to be in the same room as started in");

        }
    }
}

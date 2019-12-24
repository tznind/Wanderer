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
                .Returns(new Room(world))
                .Returns(new Room(world))
                .Returns(new Room(world))
                .Throws<Exception>();
            
            world.Player = new You();
            world.RoomFactory = factory.Object;
            world.CurrentLocation = factory.Object.Create(world);

            var leave = new Leave(world);
            var uiGoSouth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.South);
            var uiGoNorth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.North);

            var room1 = world.CurrentLocation = factory.Object.Create(world);
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            leave.Push(uiGoSouth,stack);
            leave.Pop(uiGoSouth,stack);
            
            var room2 = world.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");

            leave.Push(uiGoNorth,stack);
            
            leave.Pop(uiGoNorth,stack);
            

            Assert.AreSame(room1, world.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var world = new World(new You(),null);
            var room1 = world.CurrentLocation = new Room(world){Title = "Hotel California"};

            var guard = new Actor("Guard");

            guard.AddBehaviour(new ForbidBehaviour<Leave>((s)=>true));

            world.CurrentLocation = room1;

            var leave = new Leave(world);
            var ui = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.South);

            var stack = new ActionStack();

            leave.Push(ui,stack);

            foreach (var finalBehaviour in guard.GetFinalBehaviours()) 
                finalBehaviour.OnPush(ui,stack);
            
            while(stack.TryPop(out IAction action))
                action.Pop(ui, stack);

            Assert.AreSame(room1,world.CurrentLocation,"Expected to be in the same room as started in");

        }
    }

    
}

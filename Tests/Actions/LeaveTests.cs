using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Places;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Actions
{
    class LeaveTests
    {
        [Test]
        public void LeaveAndComeBack()
        {
            var world = new World(new You());

            var factory = new Mock<IRoomFactory>();
            factory.Setup(f => f.Create(world)).Returns(new Room(world));

            var leave = new Leave(world);
            var uiGoSouth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.South);
            var uiGoNorth = Mock.Of<IUserinterface>(u => u.GetOption<Direction>(It.IsAny<string>()) == Direction.North);

            var room1 = world.CurrentLocation = factory.Object.Create(world);
            Assert.IsNotNull(room1);

            leave.Perform(uiGoSouth);

            var room2 = world.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.AreNotSame(room1, room2);

            leave.Perform(uiGoNorth);

            Assert.AreSame(room1, world.CurrentLocation,"Should be back in the first room again");
        }
    }
}

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
    public class M
    {
       public static IUserinterface UI_GetChoice<T>(T choice)
       {
           var log = new EventLog();
           log.Register();

            T outvar = choice;
            return Mock.Of<IUserinterface>(u => 
                u.GetChoice<T>(It.IsAny<string>(),It.IsAny<string>(),out outvar,It.IsAny<T[]>()) == true
                && u.Log == log);
       }
    }


    class LeaveTests
    {
        

        [Test]
        public void LeaveAndComeBack()
        {

            var factory = new Mock<IRoomFactory>();
            var world = new World();

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room("Starting Room",world))
                .Returns(new Room("North Room",world))
                .Throws<Exception>();
            
            world.Player = new You(factory.Object.Create(world));
            world.RoomFactory = factory.Object;
            world.Map.Add(new Point3(0,0,0), world.Player.CurrentLocation);
            
            Assert.AreEqual(new Point3(0,0,0),world.Map.GetPoint(world.Player.CurrentLocation));
            
            var leave = new Leave(world.Player);

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();
            stack.RunStack(M.UI_GetChoice(Direction.North),leave,new IBehaviour[0]);
            
            var room2 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");
            
            stack.RunStack(M.UI_GetChoice(Direction.South),leave,new IBehaviour[0]);

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void WalkInCircle()
        {

            var factory = new Mock<IRoomFactory>();
            var world = new World();

            factory.SetupSequence(f => f.Create(It.IsAny<IWorld>()))
                .Returns(new Room ( "StartingRoom",world))
                .Returns(new Room("NorthRoom",world)) 
                .Returns(new Room("NorthEastRoom",world))
                .Returns(new Room( "EastRoom",world));
            //next return will be null and should not be invoked.  After all going West
            //from the EastRoom should result in being back in StartingRoom
            
            world.Player = new You(factory.Object.Create(world));
            world.RoomFactory = factory.Object;
            world.Map.Add(new Point3(0,0,0), world.Player.CurrentLocation);

            var leave = new Leave(world.Player);

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            //north
            stack.RunStack(M.UI_GetChoice(Direction.North),leave,new IBehaviour[0]);
            Assert.AreNotSame(room1, world.Player.CurrentLocation,"After going north we should not be in the same room");

            //east,south,west
            stack.RunStack(M.UI_GetChoice(Direction.East),leave,new IBehaviour[0]);
            stack.RunStack(M.UI_GetChoice(Direction.South),leave,new IBehaviour[0]);
            stack.RunStack(M.UI_GetChoice(Direction.West),leave,new IBehaviour[0]);

            factory.Verify();

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var world = new World();
            var room = new Room("Hotel California",world);
            world.Player = new You(room);
            
            var guard = new Npc("Guard",room);

            guard.AddBehaviour(new ForbidBehaviour<Leave>(new AlwaysCondition<IAction>(),guard));
            
            var leave = new Leave(world.Player);

            var stack = new ActionStack();

            stack.RunStack(M.UI_GetChoice(Direction.South), leave, guard.GetFinalBehaviours());

            Assert.AreSame(room,world.Player.CurrentLocation,"Expected to be in the same room as started in");

        }
    }
}

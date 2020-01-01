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

    /// <summary>
    /// Test helper class which handles calls to <see cref="IUserinterface.GetChoice{T}"/> by returning fixed values in order
    /// (uses out parameter so cannot easily use SetupSequence)
    /// </summary>
    class GetChoiceTestUI : IUserinterface
    {
        private readonly object[] _getChoiceReturns;
        private int _index;
        public EventLog Log { get; } = new EventLog();

        public GetChoiceTestUI(params object[] getChoiceReturns)
        {
            _getChoiceReturns = getChoiceReturns;
            _index = 0;
            Log.Register();
        }

        public void ShowActorStats(IActor actor)
        {
            throw new NotImplementedException();
        }

        public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            if(_index >= _getChoiceReturns.Length)
                throw new Exception($"Did not have an answer for GetChoice of:{title} ({body})");

            chosen = (T) _getChoiceReturns[_index++];

            if(!options.Contains(chosen))
                throw new Exception($"Chosen test answer was not one of the listed options for GetChoice of:{title} ({body})");
            return true;
        }

        public void Refresh()
        {
        }

        public void ShowMessage(string title, string body, bool log, Guid round)
        {
            if(log)
                Log.Info(body,round);
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
            
            world.Population.Add(new You("TestPlayer",factory.Object.Create(world)));
            world.RoomFactory = factory.Object;
            world.Map.Add(new Point3(0,0,0), world.Player.CurrentLocation);
            
            Assert.AreEqual(new Point3(0,0,0),world.Map.GetPoint(world.Player.CurrentLocation));
            
            var leave = new Leave();

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();
            stack.RunStack(M.UI_GetChoice(Direction.North),leave,world.Player,new IBehaviour[0]);
            
            var room2 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);
            Assert.IsNotNull(room2);
            Assert.AreNotSame(room1, room2,"Expected room to change after leaving it");
            
            stack.RunStack(M.UI_GetChoice(Direction.South),leave,world.Player,new IBehaviour[0]);

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
            
            world.Population.Add(new You("TestPlayer",factory.Object.Create(world)));
            world.RoomFactory = factory.Object;
            world.Map.Add(new Point3(0,0,0), world.Player.CurrentLocation);

            var leave = new Leave();

            var room1 = world.Player.CurrentLocation;
            Assert.IsNotNull(room1);

            var stack = new ActionStack();

            //north
            stack.RunStack(M.UI_GetChoice(Direction.North),leave,world.Player,new IBehaviour[0]);
            Assert.AreNotSame(room1, world.Player.CurrentLocation,"After going north we should not be in the same room");

            //east,south,west
            stack.RunStack(M.UI_GetChoice(Direction.East),leave,world.Player,new IBehaviour[0]);
            stack.RunStack(M.UI_GetChoice(Direction.South),leave,world.Player,new IBehaviour[0]);
            stack.RunStack(M.UI_GetChoice(Direction.West),leave,world.Player,new IBehaviour[0]);

            factory.Verify();

            Assert.AreSame(room1, world.Player.CurrentLocation,"Should be back in the first room again");
        }

        [Test]
        public void CannotLeave()
        {
            var world = new World();
            var room = new Room("Hotel California",world);
            world.Population.Add(new You("TestPlayer",room));
            
            var guard = new Npc("Guard",room);
            guard.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new AlwaysCondition<Frame>(),guard));
            
            var leave = new Leave();

            var stack = new ActionStack();

            stack.RunStack(M.UI_GetChoice(Direction.South), leave,world.Player ,guard.GetFinalBehaviours());

            Assert.AreSame(room,world.Player.CurrentLocation,"Expected to be in the same room as started in");

        }
    }
}

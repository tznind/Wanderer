using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Places;

namespace Tests.Places
{
    public class DijstraPathingTests
    {
        [Test]
        public void TestDijkstraPathing()
        {

             HashSet<Direction> any = new HashSet<Direction>
            {
                Direction.North,
                Direction.East,
                Direction.South,
                Direction.West,
                Direction.Up,
                Direction.Down,
            };

            var _000 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "000");
            var _001 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "001");
            var _010 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "010");
            var _011 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "011");
            var _100 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "100");
            var _101 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "101");
            var _110 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "110");
            var _111 = Mock.Of<IRoom>(p=>p.LeaveDirections == any && p.ToString() == "111");

            var map = new Map();
            map.Add(new Point3(0,0,0),_000);
            map.Add(new Point3(0,0,1),_001);
            map.Add(new Point3(0,1,0),_010);
            map.Add(new Point3(0,1,1),_011);
            map.Add(new Point3(1,0,0),_100);
            map.Add(new Point3(1,0,1),_101);
            map.Add(new Point3(1,1,0),_110);
            map.Add(new Point3(1,1,1),_111);

            var pathing = new DijkstraPathing(map,_000,_111);

            var result = pathing.GetShortestPathDijkstra();

            Assert.AreEqual(4,result.Count);
            Assert.AreEqual(_000,result[0].Place);
            Assert.AreEqual(_001,result[1].Place);
            Assert.AreEqual(_011,result[2].Place);
            Assert.AreEqual(_111,result[3].Place);


        }
    }
}





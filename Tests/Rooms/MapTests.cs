using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Wanderer;
using Wanderer.Rooms;

namespace Tests.Rooms
{
    class MapTests : UnitTest
    {
        [Test]
        public void TestRoomNotInMap_ThrowsException()
        {
            InARoom(out IWorld w);
            var r = new Room("fff", w, '-');

            var ex = Assert.Throws<Exception>(() => r.GetPoint());

            Assert.AreEqual("Supplied Room 'fff' was not in the current Map",ex.Message);

            w.Map.Add(new Point3(6,7,8),r);

            Assert.AreEqual(6,r.GetPoint().X);
            Assert.AreEqual(7,r.GetPoint().Y);
            Assert.AreEqual(8,r.GetPoint().Z);
        }

        [Test]
        public void TestMapSerialization()
        {
            var p = new Point3(0, 1, 2);

            Assert.AreEqual("0,1,2",p.ToString());

            var world = new World();

            var room = new Room("someroom",world,'-');

            var m = new Map {{p, room}};

            var json = JsonConvert.SerializeObject(m,World.GetJsonSerializerSettings());
            
            StringAssert.Contains("0,1,2",json);

            var m2 = JsonConvert.DeserializeObject<Map>(json, World.GetJsonSerializerSettings());

            Assert.AreEqual("someroom",m2[new Point3(0,1,2)].Name);
        }


        [Test]
        public void TestAdjacent_IgnorePathing()
        {

             HashSet<Direction> onlyNorth = new HashSet<Direction>
            {
                Direction.North,
            };

            var _000 = Mock.Of<IRoom>(p=>p.LeaveDirections == onlyNorth );
            var _010 = Mock.Of<IRoom>();
            var _001 = Mock.Of<IRoom>();
            var _003 = Mock.Of<IRoom>();

            var map = new Map();
            map.Add(new Point3(0,0,0),_000);
            map.Add(new Point3(0,1,0),_010);
            map.Add(new Point3(0,0,1),_001);
            map.Add(new Point3(0,0,3),_003);

            var result = map.GetAdjacentRooms(_000,false);

            Assert.AreEqual(2,result.Count);
            Assert.AreEqual(_010,result[Direction.North]);
            Assert.AreEqual(_001,result[Direction.Up]);
        }


        [Test]
        public void TestAdjacent_RespectPathing()
        {
             HashSet<Direction> onlyNorth = new HashSet<Direction>
            {
                Direction.North,
            };

            var _000 = Mock.Of<IRoom>(p=>p.LeaveDirections == onlyNorth );
            var _010 = Mock.Of<IRoom>();
            var _001 = Mock.Of<IRoom>();
            var _003 = Mock.Of<IRoom>();

            var map = new Map();
            map.Add(new Point3(0,0,0),_000);
            map.Add(new Point3(0,1,0),_010);
            map.Add(new Point3(0,0,1),_001);
            map.Add(new Point3(0,0,3),_003);

            var result = map.GetAdjacentRooms(_000,true);

            Assert.AreEqual(1,result.Count);
            Assert.AreEqual(_010,result[Direction.North]);
        }
    }
}

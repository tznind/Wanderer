using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Wanderer;
using Wanderer.Places;

namespace Tests.Places
{
    class MapTests : UnitTest
    {
        [Test]
        public void TestRoomNotInMap_ThrowsException()
        {
            InARoom(out IWorld w);
            var r = new Room("fff", w, '-');

            var ex = Assert.Throws<Exception>(() => r.GetPoint());

            Assert.AreEqual("Supplied Place 'fff' was not in the current Map",ex.Message);

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
    }
}

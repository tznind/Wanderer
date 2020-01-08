using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Places;

namespace Tests.Places
{
    class MapTests
    {
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

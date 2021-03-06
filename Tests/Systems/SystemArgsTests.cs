using System;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Tests.Systems
{
    public class SystemArgsTests : UnitTest
    {

        [Test]
        public void TestSystemArgs_Properties()
        {
            TwoInARoomWithRelationship(100,false,out You you, out IActor them,out IWorld world);

            var args = new SystemArgs(world,GetUI(),0,you,them,Guid.Empty);

            Assert.AreEqual(you.CurrentLocation,args.Room);
            Assert.AreEqual(100,them.AttitudeTo(you));

            Assert.AreEqual(0,you.AttitudeTo(them));
        }
    }
} 

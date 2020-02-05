using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Compilation;
using StarshipWanderer.Places;
using StarshipWanderer.Systems;

namespace Tests.CompilerTests
{
    class PropertyChainTests
    {
        [Test]
        public void Test_UnknownProperty()
        {
            var chain = new PropertyChain("Fish");

            var ex = Assert.Throws<PropertyChainException>(()=>chain.FollowChain(null));
            Assert.AreEqual("Value passed into FollowChain was null",ex.Message);
            
            ex = Assert.Throws<PropertyChainException>(()=>chain.FollowChain("Trollol"));
            StringAssert.Contains("Failure following PropertyChain ('Fish') for root object Trollol.  Failing link was 'Fish'",ex.Message);
        }

        
        [Test]
        public void Test_WrongType()
        {
            var chain = new PropertyChain("Name");

            var ex = Assert.Throws<PropertyChainException>(()=>chain.FollowChain(Mock.Of<IActor>(a => a.Name == "ff")));
            StringAssert.Contains("Final leaf of PropertyChain ('Name') was not an IHasStats",ex.Message);
            
        }

        
        [Test]
        public void Test_NullLink()
        {
            var chain = new PropertyChain("Items.Count");

            var ex = Assert.Throws<PropertyChainException>(()=>chain.FollowChain(Mock.Of<IActor>()));
            StringAssert.Contains("Encountered null value without exhausting PropertyChain ('Items->Count')",ex.Message);            
        }

        
        [Test]
        public void Test_Valid()
        {
            var p = Mock.Of<IPlace>();
            var a = Mock.Of<IActor>(x=>x.CurrentLocation == p);
            var chain = new PropertyChain("AggressorIfAny.CurrentLocation");
            var args = new SystemArgs(null, 0, a, null, Guid.Empty);

            Assert.AreEqual(p,chain.FollowChain(args));
        }

    }
}

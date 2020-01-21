using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Factories;

namespace Tests.Actors
{
    class NameFactoryTests
    {
        [Test]
        public void Test_BothNames()
        {
            var name = new NameFactory(new[] {"Franky"}, new[] {"Romero"}).GenerateName(new Random(32));
            Assert.AreEqual("Franky Romero",name);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Test_ForenameOnly(bool empty)
        {
            var name = new NameFactory(new[] {"Franky"}, empty ? new string[0]:null).GenerateName(new Random(32));
            Assert.AreEqual("Franky",name);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Test_SurnameOnly(bool empty)
        {
            var name = new NameFactory(empty ? new string[0]:null, new[] {"Romero"}).GenerateName(new Random(32));
            Assert.AreEqual("Romero",name);
        }


        [Test]
        public void Test_Neither()
        {
            var name = new NameFactory(null,null).GenerateName(new Random(32));
            Assert.AreEqual("",name);
        }
    }
}

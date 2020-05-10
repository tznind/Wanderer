using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Relationships;

namespace Tests.Actors
{
    class NameFactoryTests
    {
        [Test]
        public void Test_BothNames()
        {
            var name = new Faction
            {
                Forenames = new[] {"Franky"},
                Surnames = new[] {"Romero"}
            }.GenerateName(new Random(32));
            Assert.AreEqual("Franky Romero",name);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Test_ForenameOnly(bool empty)
        {
            var name = new Faction(){
                Forenames = new[] {"Franky"},
                Surnames = empty ? new string[0]:null

            }.GenerateName(new Random(32));

            Assert.AreEqual("Franky",name);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Test_SurnameOnly(bool empty)
        {
            var name = new Faction(){Forenames = empty ? new string[0]:null, 
                Surnames = new[] {"Romero"}}
                .GenerateName(new Random(32));
            Assert.AreEqual("Romero",name);
        }


        [Test]
        public void Test_Neither()
        {
            var name = new Faction().GenerateName(new Random(32));
            Assert.AreEqual("",name);
        }
    }
}

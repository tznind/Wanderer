using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Stats;

namespace Tests.Adjectives
{
    class AdjectiveTests : UnitTest
    {

        [Test]
        public void Test_AdjectiveEquality()
        {
            var a1 = new Adjective(Mock.Of<IActor>()){Name = "Attractive"};

            var a2 = new Adjective(Mock.Of<IActor>()){Name = "Attractive"};
            Assert.AreNotEqual(a1,a2);

            var ac1 = new List<Adjective> {a1};

            var ac2 = new List<Adjective> {a2};

            //they are not equal but the user would consider them identical collections
            Assert.IsTrue(ac1.AreIdentical(ac2));
            Assert.AreNotEqual(ac1,ac2);
        }

        [Test]
        public void Test_StatsRatio_Item()
        {
            var you = YouInARoom(out _);
            you.BaseStats["Beauty"] = 5;

            var watch = new Item("Watch").With("Beauty", 30);
            
            Assert.AreEqual(30,watch.GetFinalStats(you)["Beauty"]);
            you.Items.Add(watch);
            Assert.AreEqual(35,you.GetFinalStats()["Beauty"]);

            watch.Adjectives.Add(new Adjective(watch)
            {
                Name = "tarnished",
                StatsRatio = new StatsCollection(0.5)
            });

            Assert.AreEqual(15,watch.GetFinalStats(you)["Beauty"]);
            Assert.AreEqual(20,you.GetFinalStats()["Beauty"]);
        }
        [Test]
        public void Test_StatsRatio_Room()
        {
            var you = YouInARoom(out _);
            you.CurrentLocation.BaseStats["Beauty"] = 10;
            you.BaseStats["Beauty"] = 3;

            you.CurrentLocation.Adjectives.Add(new Adjective(you)
            {
                Name = "tarnished",
                StatsRatio = new StatsCollection(0.5)
            });

            Assert.AreEqual(5, you.CurrentLocation.GetFinalStats(you)["Beauty"] );
            Assert.AreEqual(8, you.GetFinalStats(you)["Beauty"] ,"expected your final stats to be your base stats plus the (modified) room stats");
        }

        [Test]
        public void Test_StatsRatio_Actor()
        {
            var you = YouInARoom(out _);
            you.CurrentLocation.BaseStats["Beauty"] = 10;
            you.BaseStats["Beauty"] = 6;

            //you are tarnished 
            you.Adjectives.Add(new Adjective(you)
            {
                Name = "tarnished",
                StatsRatio = new StatsCollection(0.5)
            });

            Assert.AreEqual(13, you.GetFinalStats()["Beauty"], "Expected your (modified) base stats (3) plus the rooms unmodified stats (10)");
        }
    }
}

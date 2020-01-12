using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace Tests.Items
{
    class MoneyTests
    {
        [Test]
        public void Test_RustyReducesValue()
        {
            var itemFactory = new ItemFactory(new AdjectiveFactory());

            var you = new You("Dave", new Room("SomeRoom",new World(), 'f'));
            
            Assert.Less(
            itemFactory.Create<Rusty>("Cog").With(Stat.Value,10).GetFinalStats(you)[Stat.Value],
            new Item("Cog").With(Stat.Value,10).GetFinalStats(you)[Stat.Value]);
        }

        [Test]
        public void Test_MoneyStacking()
        {
            var world = new World();
            var you = new You("Dave", new Room("SomeRoom",world, 'f'));

            var money = new ItemStack("Money", 10).With(Stat.Value, 1);
            
            Assert.AreEqual(10,money.GetFinalStats(you)[Stat.Value],"Money has worth depending on stack size");
            Assert.AreEqual(0,money.GetFinalStats(you)[Stat.Fight],"Money doesn't help you fight");
        }

        [Test]
        public void Test_RustyMoney()
        {
            var world = new World();
            var you = new You("Dave", new Room("SomeRoom",world, 'f'));

            var factory = new ItemFactory(new AdjectiveFactory());
            var money = factory.CreateStack<Rusty>("Money",10).With(Stat.Value, 1);
            
            Assert.AreEqual(5,money.GetFinalStats(you)[Stat.Value],"Money has worth depending on stack size");
        }
    }

}

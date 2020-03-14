using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Stats;
using Tests.Actions;

namespace Tests.Items
{
    class MoneyTests : UnitTest
    {
        [Test]
        public void Test_RustyReducesValue()
        {
            var you = new You("Dave", new Room("SomeRoom",new World(), 'f'));

            var cog = new Item("Cog").With(Stat.Value, 10);
            cog.Adjectives.Add( new Adjective(cog)
            {
                Name = "Rusty", StatsRatio = {[Stat.Value] = 0.5},
            });

            Assert.AreEqual(5, cog.GetFinalStats(you)[Stat.Value]);
        }
        
        [Test]
        public void Test_RustyDoesNotImproveNegativeValue()
        {
            var you = new You("Dave", new Room("SomeRoom",new World(), 'f'));

            var cog = new Adjective(new Item("CursedCog").With(Stat.Value, -10))
            {
                Name = "Rusty", StatsRatio = {[Stat.Value] = 0.5},
            };

            Assert.AreEqual(-20, cog.GetFinalStats(you)[Stat.Value]);
        }
        [Test]
        public void Test_MoneyStacking()
        {
            var world = new World();
            var room = new Room("SomeRoom", world, 'f');
            var you = new You("Dave", room);
            world.Map.Add(new Point3(0,0,0),room);

            var money = new ItemStack("Money", 10).With(Stat.Value, 1);
            room.Items.Add(money);

            Assert.AreEqual(10,money.GetFinalStats(you)[Stat.Value],"Money has worth depending on stack size");
            Assert.AreEqual(0,money.GetFinalStats(you)[Stat.Fight],"Money doesn't help you fight");

            var money2 = new ItemStack("Money", 10).With(Stat.Value, 1);
            room.Items.Add(money2);

            //pick both up
            world.RunRound(new FixedChoiceUI((IItem)money),you.GetFinalActions().OfType<PickUpAction>().Single());
            world.RunRound(new FixedChoiceUI((IItem)money2),you.GetFinalActions().OfType<PickUpAction>().Single());

            // you only have 1 item which has all the Money
            Assert.AreSame(money,you.Items.Single());
            Assert.AreEqual(20,money.StackSize);
            Assert.AreEqual(20,money.GetFinalStats(you)[Stat.Value]);


        }

        [Test]
        public void Test_RustyMoney()
        {
            var world = new World();
            var you = new You("Dave", new Room("SomeRoom",world, 'f'));

            var money = new ItemStack("money",10).With(Stat.Value, 1);
            money.Adjectives.Add(new Adjective(money)
            {
                Name = "Rusty",
                IsPrefix = true,
                StatsRatio = new StatsCollection(0.5)
            }) ;
            
            Assert.AreEqual(5,money.GetFinalStats(you)[Stat.Value],"Money has worth depending on stack size");
        }
    }

}

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
    }

}

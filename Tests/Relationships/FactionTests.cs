using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Stats;

namespace Tests.Relationships
{
    class FactionTests : UnitTest
    {
        [Test]
        public void BeingInFactionGrantsStats()
        {
            var you = YouInARoom(out IWorld _);

            var f = new Faction();
            f.Name = "Geniuses";
            f.BaseStats[Stat.Savvy] = 50;
            
            var before = you.GetFinalStats()[Stat.Savvy];

            you.FactionMembership.Add(f);

            Assert.AreEqual(before + 50, you.GetFinalStats()[Stat.Savvy]);
            you.FactionMembership.Clear();
            
            Assert.AreEqual(before, you.GetFinalStats()[Stat.Savvy]);

        }

        [Test]
        public void BeingInFactionGrantsAdjectives()
        {
            var you = YouInARoom(out IWorld _);
            you.BaseStats[Stat.Savvy] = 50; //allows use of medic skill

            var f = new Faction();
            f.Name = "Medical Corp";
            f.Adjectives.Add(new Medic(f));
            
            Assert.IsFalse(you.Has<Medic>(false));

            you.FactionMembership.Add(f);
            
            Assert.IsTrue(you.Has<Medic>(false));
            Assert.AreEqual(1,you.GetFinalActions().OfType<HealAction>().Count());
            
            you.FactionMembership.Clear();

            Assert.IsFalse(you.Has<Medic>(false));

        }
    }
}

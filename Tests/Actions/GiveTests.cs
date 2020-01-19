﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace Tests.Actions
{
    class GiveTests : UnitTest
    {
        [Test]
        public void Test_GiveItem()
        {
            var you = YouInARoom(out IWorld world);

            var them = new Npc("Mort", you.CurrentLocation);
            them.BaseActions.Clear();

            var grenade = new Item("Grenade");
            you.Items.Add(grenade);

            //when you chose to give them the grenade
            world.RunRound(GetUI(grenade, them), you.BaseActions.OfType<GiveAction>().Single());

            Assert.IsFalse(you.Items.Contains(grenade));
            Assert.Contains(grenade,them.Items.ToArray());
        }

        [Test]
        public void TestGiveItem_ValueAffectsRelationship()
        {
            var you = YouInARoom(out IWorld world);

            var them = new Npc("Mort", you.CurrentLocation);
            them.BaseActions.Clear();

            var smallStackOfCash = new ItemStack("Jingle",20).With(Stat.Value,1);
            var bigStackOfCash = new ItemStack("Jingle",40).With(Stat.Value,1);
            
            you.Items.Add(smallStackOfCash);
            you.Items.Add(bigStackOfCash);

            var relationship = world.Relationships.OfType<PersonalRelationship>().SingleOrDefault(r=>r.AppliesTo(them, you));
            Assert.IsNull(relationship,"Expected no relationship between you");

            //when you chose to give them the small stack
            world.RunRound(GetUI(smallStackOfCash, them), you.BaseActions.OfType<GiveAction>().Single());

            var afterSmall = world.Relationships.OfType<PersonalRelationship>().Single(r=>r.AppliesTo(them, you)).Attitude;
            Assert.AreEqual(20,afterSmall);

            //when you chose to give them the small stack
            world.RunRound(GetUI(bigStackOfCash, them), you.BaseActions.OfType<GiveAction>().Single());

            var afterBig = world.Relationships.OfType<PersonalRelationship>().Single(r=>r.AppliesTo(them, you)).Attitude;
            Assert.AreEqual(60,afterBig);
        }
    }
}

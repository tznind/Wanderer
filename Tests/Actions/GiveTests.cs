using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Wanderer.Stats;

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

            Assert.IsFalse(new GiveAction(you).HasTargets(you));

            var grenade = new Item("Grenade");
            you.Items.Add(grenade);
            grenade.BaseActions.Clear();
            grenade.BaseActions.Add(new GiveAction(grenade));
            
            Assert.IsTrue(new GiveAction(grenade).HasTargets(you));

            //when you chose to give them the grenade
            world.RunRound(GetUI(grenade, them), you.GetFinalActions().OfType<GiveAction>().Single());

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
            world.RunRound(GetUI(smallStackOfCash, them), you.GetFinalActions().OfType<GiveAction>().First());

            var afterSmall = world.Relationships.OfType<PersonalRelationship>().Single(r=>r.AppliesTo(them, you)).Attitude;
            Assert.AreEqual(20,afterSmall);

            //when you chose to give them the small stack
            world.RunRound(GetUI(bigStackOfCash, them), you.GetFinalActions().OfType<GiveAction>().First());

            var afterBig = world.Relationships.OfType<PersonalRelationship>().Single(r=>r.AppliesTo(them, you)).Attitude;
            Assert.AreEqual(60,afterBig);
        }
    }
}

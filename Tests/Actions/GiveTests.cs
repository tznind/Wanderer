using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

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
    }
}

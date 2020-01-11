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
    class GiveTests
    {
        [Test]
        public void Test_GiveItem()
        {
            var world = new World();
            var room = new Room("Somewhere",world , 't');
            world.Map.Add(new Point3(0,0,0),room);
            var you = new You("Wanderer", room);
            var them = new Npc("Mort", room);
            them.BaseActions.Clear();

            var grenade = new Item("Grenade");
            you.Items.Add(grenade);

            //when you chose to give them the grenade
            world.RunRound(new GetChoiceTestUI(grenade, them), you.BaseActions.OfType<GiveAction>().Single());

            Assert.IsFalse(you.Items.Contains(grenade));
            Assert.Contains(grenade,them.Items.ToArray());
        }
    }
}

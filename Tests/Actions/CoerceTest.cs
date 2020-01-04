using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace Tests.Actions
{
    class CoerceTest
    {
        [Test]
        public void Test_CoerceSuccess_PerformsAction()
        {
            var world = new World();

            var room = new Room("TestRoom", world);
            world.Map.Add(new Point3(0,0,0),room );
            var you = new You("Test player",room);

            //create two npcs that can both fight
            //a acts before b so coercion allows him to kill b before he can act
            var a = new Npc("A", room);
            a.BaseStats[Stat.Initiative] = 10;

            var b = new Npc("B", room);
            b.BaseStats[Stat.Initiative] = 0;

            //put everyone in a room together
            world.Population.Add(you);
            world.Population.Add(a);
            world.Population.Add(b);

            var ui = new GetChoiceTestUI(a,a.GetFinalActions().OfType<FightAction>().Single(),b);
            world.RunRound(ui,world.Player.BaseActions.OfType<CoerceAction>().Single());
            
            //a should have killed b
            Assert.Contains(a,world.Population.ToArray());
            Assert.IsFalse(world.Population.Contains(b));

            Assert.Contains("Test player coerced A to perform Fight", ui.Log.RoundResults);
            Assert.Contains("A fought and killed B", ui.Log.RoundResults);
            Assert.AreEqual(2,ui.Log.RoundResults.Count);

            //initiative should have been boosted to do the coerce then reset at end of round
            Assert.AreEqual(10,a.GetFinalStats()[Stat.Initiative]);
        }
    }
}

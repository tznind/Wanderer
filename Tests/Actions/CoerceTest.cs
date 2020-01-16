using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace Tests.Actions
{
    class CoerceTest : UnitTest
    {
        [Test]
        public void Test_CoerceSuccess_PerformsAction()
        {
            var you = YouInARoom(out IWorld world);

            //create two npcs that can both fight
            var a = new Npc("A",you.CurrentLocation).With(Stat.Initiative,10);
            var b = new Npc("B",you.CurrentLocation).With(Stat.Initiative,0);
            
            Assert.IsFalse(b.Has<Injured>(false));

            var ui = GetUI(a, a.GetFinalActions().OfType<FightAction>().Single(), b);
            world.RunRound(ui,world.Player.BaseActions.OfType<CoerceAction>().Single());
            
            //a should have been injured
            Assert.Contains(a,world.Population.ToArray());
            Assert.IsTrue(b.Has<Injured>(false));

            Assert.Contains("Test Wanderer coerced A to perform Fight", ui.Log.RoundResults.Select(r=>r.Message).ToArray());
            
            //initiative should have been boosted to do the coerce then reset at end of round
            Assert.AreEqual(10,a.GetFinalStats()[Stat.Initiative]);
        }
    }
}

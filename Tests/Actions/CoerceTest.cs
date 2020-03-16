using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actions.Coercion;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Stats;

namespace Tests.Actions
{
    class CoerceTest : UnitTest
    {

        [Test]
        public void Test_CoerceCancelling()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var ui = GetUI(null);

            Assert.IsFalse(new ActionStack().RunStack(world,ui,new CoerceAction(), you,null));
            
            ui = GetUI(them, null);
            Assert.IsFalse(new ActionStack().RunStack(world,ui,new CoerceAction(), you,null));

            //Its too late to cancel.  You have successfully coerced you have to pick targets for the NPC on their go
            ui = GetUI(them, them.GetFinalActions().OfType<FightAction>().Single(), null);
            Assert.IsTrue(new ActionStack().RunStack(world,ui,new CoerceAction(), you,null));
            
            ui = GetUI(them, them.GetFinalActions().OfType<FightAction>().Single(), you);
            world.RunRound(ui, new CoerceAction());
            Assert.IsTrue(ui.IsExhausted);

            //can't get them to hit themselves!
            ui = GetUI(them, them.GetFinalActions().OfType<FightAction>().Single(), them);
            Assert.Throws<OptionNotAvailableException>(()=>world.RunRound(ui,new CoerceAction()));

        }

        [Test]
        public void Test_CoerceSuccess_PerformsAction()
        {
            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Coerce] = 100;

            Assert.IsFalse(new CoerceAction().HasTargets(you));

            //create two npcs that can both fight
            var a = new Npc("A",you.CurrentLocation).With(Stat.Initiative,10);
            var b = new Npc("B",you.CurrentLocation).With(Stat.Initiative,0);
            
            Assert.IsTrue(new CoerceAction().HasTargets(you));
            Assert.IsFalse(b.Has("Injured",false));

            var ui = GetUI(a, a.GetFinalActions().OfType<FightAction>().Single(), b);
            world.RunRound(ui,world.Player.BaseActions.OfType<CoerceAction>().Single());
            
            //a should have been injured
            Assert.Contains(a,world.Population.ToArray());
            Assert.IsTrue(b.Has("Injured",false));

            Assert.Contains("Test Wanderer coerced A to perform Fight", ui.Log.RoundResults.Select(r=>r.Message).ToArray());
            
            //initiative should have been boosted to do the coerce then reset at end of round
            Assert.AreEqual(10,a.GetFinalStats()[Stat.Initiative]);
        }

        [Test]
        public void TestCoerce_RelativeDifficulty()
        {
            TwoInARoom(out You you,out IActor them,out IWorld w);

            you.BaseStats[Stat.Coerce] = 20;
            var copper = new Item("Copper Coin").With(Stat.Value, 1);
            them.Items.Add(copper);
            var giveCopper = GetUI(them, them.GetFinalActions().OfType<GiveAction>().Single(), copper, you);
            w.RunRound(giveCopper,new CoerceAction());

            //when you coerce them to give you something cheap it works
            Assert.Contains(copper,you.Items.ToArray());
            Assert.Contains("Chaos Sam gave Copper Coin to Test Wanderer",giveCopper.Log.RoundResults.Select(m=>m.Message).ToArray());
            

            var platinum = new Item("Platinum Coin").With(Stat.Value, 100);
            them.Items.Add(platinum);
            var givePlatinum = GetUI(them, them.GetFinalActions().OfType<GiveAction>().Single(), platinum, you);
            w.RunRound(givePlatinum,new CoerceAction());

            //when you coerce them to give you something expensive it doesnt work
            Assert.IsFalse(you.Items.Contains(platinum),"Expected them to refuse to give you the platinum");
            Assert.IsTrue(them.Items.Contains(platinum));
            Assert.Contains("Test Wanderer failed to coerce Chaos Sam - Insufficient persuasion (Needed 120, Had 20)",givePlatinum.Log.RoundResults.Select(m=>m.Message).ToArray());

        }

        [Test]
        public void CoerceFriends_IsEasier()
        {
            TwoInARoom(out You you,out IActor them,out IWorld w);

            var platinum = new Item("Platinum Coin").With(Stat.Value, 100);
            them.Items.Add(platinum);
            var givePlatinum = GetUI(them, them.GetFinalActions().OfType<GiveAction>().Single(), platinum, you);
            w.RunRound(givePlatinum,new CoerceAction());

            //when you coerce them to give you something expensive it doesn't work
            Assert.IsFalse(you.Items.Contains(platinum),"Expected them to refuse to give you the platinum");
            Assert.IsTrue(them.Items.Contains(platinum));
            Assert.Contains("Test Wanderer failed to coerce Chaos Sam - Insufficient persuasion (Needed 110, Had 10)",givePlatinum.Log.RoundResults.Select(m=>m.Message).ToArray());

            //they love you!
            w.Relationships.Add(new PersonalRelationship(them,you){Attitude=110});

            //try now
            givePlatinum = GetUI(them, them.GetFinalActions().OfType<GiveAction>().Single(), platinum, you);
            w.RunRound(givePlatinum,new CoerceAction());

            Assert.IsTrue(you.Items.Contains(platinum),"Expected them to give you the platinum now you are friends");
            Assert.IsFalse(them.Items.Contains(platinum));


        }
    }
}

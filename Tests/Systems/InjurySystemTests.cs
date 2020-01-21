using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.ActorOnly;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Factories;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;
using Tests.Actions;
using Enumerable = System.Linq.Enumerable;

namespace Tests.Systems
{
    class InjurySystemTests : UnitTest
    {
        [Test]
        public void Test_AllInjuriesUnique()
        {
            List<string> seenSoFar = new List<string>();

            var sys = new InjurySystem();

            var world = new World();
            var room = new Room("someRoom", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var a = new You("You", room);

            
            for (int i = 0; i < 60; i++)
            {
                a.Adjectives.Clear();
                sys.Apply(new SystemArgs(GetUI(),i,null,a,Guid.Empty));

                //it should have applied 1 injury
                Assert.AreEqual(1,a.Adjectives.Count);
                var injury = (Injured)a.Adjectives.Single();

                Assert.AreNotEqual(InjuryRegion.None,injury);

                var newInjury = injury.Name;
                Assert.IsFalse(seenSoFar.Contains(newInjury),$"Saw duplicate injury '{newInjury}' for i={i}");
                seenSoFar.Add(newInjury);
            }
        }

        [Test]
        public void Test_LightInjuriesHealOverTime()
        {
            var world = new World();
            var room = new Room("someRoom", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var a = new You("You", room);
            
            //give them an injury
            var injury = new Injured("Bruised Shin", a, 1, InjuryRegion.Leg,world.InjurySystems.First());
            a.Adjectives.Add(injury);
            
            for (int i = 0; i < 10; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(GetUI(typeof(object)), new LoadGunsAction(), a, a.GetFinalBehaviours());

                //after 9 round you should still be injured                
                if(i <9)
                    Assert.Contains(injury,a.Adjectives.ToArray());
                else
                    //on 10th round it should be gone
                    Assert.IsFalse(a.Adjectives.Contains(injury));
            }
        }

        [TestCase(true,true)]
        [TestCase(true, false)]
        [TestCase(false,true)]
        [TestCase(false,false)]
        public void Test_HeavyInjuriesGetWorseOverTime(bool isTough, bool roomIsStale)
        {
            var world = new World();
            var room = new Room("someRoom", world,'-');
            world.Map.Add(new Point3(0,0,0), room);
            
            if (roomIsStale)
                room.Adjectives.Add(new Stale(room));

            var a = new You("You", room);

            //give them an injury
            var injury = new Injured("Cut Lip", a, 2, InjuryRegion.Leg,world.InjurySystems.First());
            a.Adjectives.Add(injury);

            if (isTough)
                a.Adjectives.Add(new Tough(a));

            for (int i = 0; i < 10; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(GetUI(typeof(object)), new LoadGunsAction(), a, a.GetFinalBehaviours());

                //after 2 rounds (0 and 1) you should still be injured                
                if(i == 0 )
                    StringAssert.AreEqualIgnoringCase("Cut Lip",injury.Name);
                
                if (i == 21)
                {
                    if (isTough && !roomIsStale)
                    {
                        //never gets worse
                        Assert.AreEqual(2, injury.Severity);
                    }
                    else
                    if (!roomIsStale || isTough)
                    {
                        //normal rate or tough in stale room 
                        
                        //From 2 it gets worse on the following rounds
                        //4+6+8
                        StringAssert.AreEqualIgnoringCase("Infected Cut Lip", injury.Name);
                        Assert.AreEqual(5, injury.Severity);
                    }
                    else
                    {
                        //stale room and not tough

                        //From 2 it gets worse on the following rounds
                        //2+3+4+5+6
                        StringAssert.AreEqualIgnoringCase("Infected Cut Lip", injury.Name);
                        Assert.AreEqual(7, injury.Severity);
                    }
                }
            }
        }

        [Test]
        public void Test_HealingAnInjury()
        {
            var world = new World();
            var room = new Room("someRoom", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var you = new You("You", room);

            //you are a medic
            you.Adjectives.Add(new Medic(you));
            you.BaseStats[Stat.Savvy] = 0;

            //you cannot heal even though you are a medic (because Savvy is 0)
            Assert.IsFalse(you.GetFinalActions().OfType<HealAction>().Any());

            //until you have good Savvy
            you.BaseStats[Stat.Savvy] = 50;
            
            //now you can heal stuff
            Assert.IsTrue(you.GetFinalActions().OfType<HealAction>().Any());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First());
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury,you.Adjectives.ToArray());
            stack.RunStack(new FixedChoiceUI(you,injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());
            Assert.IsFalse(you.Adjectives.Contains(injury));
        }


        [Test]
        public void Test_SevereInjuriesAreHarderToHeal()
        {
            var you = YouInARoom(out IWorld w);

            //you are a medic
            you.Adjectives.Add(new Medic(you));
            you.BaseStats[Stat.Savvy] = 20;

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,w.InjurySystems.First());
            you.Adjectives.Add(injury);
            
            var stack = new ActionStack();

            Assert.Contains(injury,you.Adjectives.ToArray());
            Assert.IsTrue(stack.RunStack(new FixedChoiceUI(you,injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));

            var badInjury = new Injured("Cut Lip", you, 8, InjuryRegion.Leg,w.InjurySystems.First());
            you.Adjectives.Add(badInjury);

            stack = new ActionStack();

            var ui = new FixedChoiceUI(you, badInjury);

            Assert.IsFalse(stack.RunStack(ui,
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
            
            Assert.Contains("Test Wanderer was unable to heal Test Wanderer's Cut Lip because Savvy was too low (required 40)",
                ui.Log.RoundResults.Select(l=>l.ToString()).ToArray());

            you.BaseStats[Stat.Savvy] = 100;

            Assert.IsTrue(stack.RunStack(new FixedChoiceUI(you, badInjury),
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
        }

        [Test]
        public void Test_GiantsAreHarderToHeal()
        {
            var you = YouInARoom(out IWorld w);

            //you are a medic
            you.Adjectives.Add(new Medic(you));
            you.BaseStats[Stat.Savvy] = 50;

            var adj = new AdjectiveFactory();
            var them = new ActorFactory(new ItemFactory(adj),adj);
            them.Add<Giant>(you);

            var badInjury = new Injured("Cut Lip", you, 8, InjuryRegion.Leg,w.InjurySystems.First());
            you.Adjectives.Add(badInjury);

            var stack = new ActionStack();

            var ui = new FixedChoiceUI(you, badInjury);
            
            Assert.IsFalse(stack.RunStack(ui,
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
            
            Assert.Contains("Test Wanderer was unable to heal Test Wanderer's Cut Lip because Savvy was too low (required 60)",
                ui.Log.RoundResults.Select(l=>l.ToString()).ToArray());

            //shrink you back down again and presto you are healed!
            you.Adjectives.Remove(you.Adjectives.OfType<Giant>().Single());

            Assert.IsTrue(stack.RunStack(new FixedChoiceUI(you, badInjury),
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));

        }

        [Test]
        public void Test_HealingAnInjury_WithSingleUseItem()
        {
            var itemFactory = new ItemFactory(new AdjectiveFactory());

            var world = new World();
            var room = new Room("someRoom", world,'-');
            world.Map.Add(new Point3(0,0,0), room);
            var you = new You("You", room);
            you.BaseStats[Stat.Savvy] = 50;
            

            //you cannot heal as a base action
            Assert.IsFalse(you.GetFinalActions().OfType<HealAction>().Any());

            //give you a kit
            var kit = itemFactory.Create<SingleUse, Medic>("Kit");
            you.Items.Add(kit);

            //now you can heal stuff
            Assert.IsTrue(you.GetFinalActions().OfType<HealAction>().Any());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First());
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury, you.Adjectives.ToArray());
            stack.RunStack(new FixedChoiceUI(you, injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());
            
            //injury is gone
            Assert.IsFalse(you.Adjectives.Contains(injury));
            Assert.IsFalse(you.Items.Contains(kit));
        }

        
        [Test]
        public void TestTooManyInjuries_IsFatal()
        {
            var you = YouInARoom(out IWorld w);
            you.BaseActions.Add(new LoadGunsAction());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,w.InjurySystems.First());
            you.Adjectives.Add(injury);

            w.RunRound(new FixedChoiceUI(),you.GetFinalActions().OfType<LoadGunsAction>().Single());

            Assert.IsFalse(you.Dead,"Did not expect you to die from light injuries");
            
            var badInjury = new Injured("Decapitated Head", you, 10, InjuryRegion.Head,w.InjurySystems.First());
            you.Adjectives.Add(badInjury);

            Assert.IsFalse(you.Dead,"Expected death check to be at the end of the round");

            w.RunRound(new FixedChoiceUI(),you.GetFinalActions().OfType<LoadGunsAction>().Single());

            Assert.IsTrue(you.Dead,"Expected you to die at the end of the round");

        }
    }
}

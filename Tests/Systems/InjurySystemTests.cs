using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Stats;
using Wanderer.Systems;
using Tests.Actions;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Serialization;
using Enumerable = System.Linq.Enumerable;

namespace Tests.Systems
{
    class InjurySystemTests : UnitTest
    {
        [Test]
        public void Test_LightInjuriesHealOverTime()
        {
            var you = YouInARoom(out IWorld world);
            var room = you.CurrentLocation;

            var a = new You("You", room);
            
            //give them an injury
            var injury = new Injured("Bruised Shin", a, 10, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            a.Adjectives.Add(injury);
            
            for (int i = 0; i < 11; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(world,GetUI(typeof(object)), new LoadGunsAction(), a, a.GetFinalBehaviours());

                //after 9 round you should still be injured                
                if(i <10)
                    Assert.Contains(injury,a.Adjectives.ToArray(), $"unexpected injury healing on round {i}");
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
            var you = YouInARoom(out IWorld world);
            var room = you.CurrentLocation;

            if (roomIsStale)
                room.Adjectives.Add(world.AdjectiveFactory.Create(room,"Stale"));

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);

            if (isTough)
                you.Adjectives.Add(world.AdjectiveFactory.Create(you,"Tough"));

            for (int i = 0; i < 10; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(world,GetUI(typeof(object)), new LoadGunsAction(), you, you.GetFinalBehaviours());

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
            var you = YouInARoom(out IWorld world);

            //you cannot heal yet
            Assert.IsFalse(you.GetFinalActions().OfType<HealAction>().Any());
            
            //you are a medic
            you.Adjectives.Add(new Adjective(you){Name = "Medic",BaseActions = {new HealAction()}});
            
            //now you can heal stuff
            Assert.IsTrue(you.GetFinalActions().OfType<HealAction>().Any());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury,you.Adjectives.ToArray());
            stack.RunStack(world,new FixedChoiceUI(you,injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());
            Assert.IsFalse(you.Adjectives.Contains(injury));
        }


        [Test]
        public void Test_SevereInjuriesAreHarderToHeal()
        {
            var you = YouInARoom(out IWorld world);

            //you are a medic
            you.Adjectives.Add(new Adjective(Mock.Of<IActor>()){Name = "Medic",BaseActions = {new HealAction()}});
            you.BaseStats[Stat.Savvy] = 20;

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);
            
            var stack = new ActionStack();

            Assert.Contains(injury,you.Adjectives.ToArray());
            Assert.IsTrue(stack.RunStack(world,new FixedChoiceUI(you,injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));

            var badInjury = new Injured("Cut Lip", you, 80, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(badInjury);

            stack = new ActionStack();

            var ui = new FixedChoiceUI(you, badInjury);

            Assert.IsFalse(stack.RunStack(world,ui,
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
            
            Assert.Contains("Test Wanderer was unable to heal Test Wanderer's Cut Lip because Savvy was too low (required 40)",
                ui.Log.RoundResults.Select(l=>l.ToString()).ToArray());

            you.BaseStats[Stat.Savvy] = 100;

            Assert.IsTrue(stack.RunStack(world,new FixedChoiceUI(you, badInjury),
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
        }

        [Test]
        public void Test_GiantsAreHarderToHeal()
        {
            var you = YouInARoom(out IWorld world);

            //you are a medic
            you.Adjectives.Add(new Adjective(you){Name = "Medic",BaseActions = {new HealAction()}});
            you.BaseStats[Stat.Savvy] = 50;
            you.With(world.AdjectiveFactory, "Giant");
            
            var badInjury = new Injured("Cut Lip", you, 80, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(badInjury);

            var stack = new ActionStack();

            var ui = new FixedChoiceUI(you, badInjury);
            
            Assert.IsFalse(stack.RunStack(world,ui,
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));
            
            Assert.Contains("Test Wanderer was unable to heal Test Wanderer's Cut Lip because Savvy was too low (required 60)",
                ui.Log.RoundResults.Select(l=>l.ToString()).ToArray());

            //shrink you back down again and presto you are healed!
            you.Adjectives.Remove(you.Adjectives.Single(a => a.Name.Equals("Giant")));

            Assert.IsTrue(stack.RunStack(world,new FixedChoiceUI(you, badInjury),
                you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours()));

        }

        [Test]
        public void Test_HealingAnInjury_WithSingleUseItem()
        {
            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Savvy] = 50;
            
            //you cannot heal as a base action
            Assert.IsFalse(you.GetFinalActions().OfType<HealAction>().Any());

            
            //give you 2 kits
            var kit1= world.ItemFactory.Create(world, new ItemBlueprint() {Name = "Kit"})
                .With(world.AdjectiveFactory,"Medic","SingleUse");
            you.Items.Add(kit1);
            var kit2 = world.ItemFactory.Create(world,new ItemBlueprint() {Name = "Kit"})
                .With(world.AdjectiveFactory,"Medic","SingleUse");
            you.Items.Add(kit2);

            //now you can heal stuff
            Assert.AreEqual(1,you.GetFinalActions().OfType<HealAction>().Count());
            //you have 2 kits
            Assert.AreEqual(2,you.Items.Count(i=>i.Name.Equals("Kit")));
            
            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury, you.Adjectives.ToArray());
            stack.RunStack(world,new FixedChoiceUI(you, injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());
            
            //injury is gone
            Assert.IsFalse(you.Adjectives.Contains(injury));
            //you have now 1 kit remaining
            Assert.AreEqual(1,you.Items.Count(i=>i.Name.Equals("Kit")));
        }


        [Test]
        public void Test_HealingAnInjury_WithSingleUseItemStack()
        {
            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Savvy] = 50;
            var kitStack = (ItemStack)world.ItemFactory.Create(world, new ItemBlueprint {Name = "Kit", Stack = 2})
                .With(world.AdjectiveFactory, "SingleUse", "Medic");
            you.Items.Add(kitStack);

            Assert.AreEqual(2,kitStack.StackSize);

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg,world.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury, you.Adjectives.ToArray());
            stack.RunStack(world,new FixedChoiceUI(you, injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());
            
            //injury is gone
            Assert.IsFalse(you.Adjectives.Contains(injury));
            //you have now 1 kit remaining
            Assert.AreEqual(1,kitStack.StackSize);

            //give them another injury
            you.Adjectives.Add(injury);
            stack.RunStack(world,new FixedChoiceUI(you, injury), you.GetFinalActions().OfType<HealAction>().Single(), you, you.GetFinalBehaviours());

            //now stack should have disapeared
            Assert.IsEmpty(you.Items);
            
        }

        [Test]
        public void Test_InjuriesDontChange_OnceDead()
        {
            var you = YouInARoom(out IWorld w);
            you.Dead = true;

            var injurySystem = w.InjurySystems.Single(i => i.Identifier == new Guid("9b137f26-834d-4033-ae36-74ab578f5868"));

            var a = new Injured("Exposed Spine", you, 7, InjuryRegion.Ribs,injurySystem);

            for (int i = 0; i < 100; i++) 
                Assert.IsFalse(a.InjurySystem.ShouldWorsen(a, i));

            
            var a2 = new Injured("Grazed Knee", you, 1, InjuryRegion.Leg,injurySystem);

            for (int i = 0; i < 100; i++)
            {
                Assert.IsFalse(a2.InjurySystem.ShouldWorsen(a, i));
                Assert.IsFalse(a2.InjurySystem.ShouldNaturallyHeal(a, i));
            }
        }
        
        [Test]
        public void TestTooManyInjuries_IsFatal()
        {
            var you = YouInARoom(out IWorld w).With(new LoadGunsAction());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 20, InjuryRegion.Leg,w.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(injury);

            w.RunRound(new FixedChoiceUI(),you.GetFinalActions().OfType<LoadGunsAction>().Single());

            Assert.IsFalse(you.Dead,"Did not expect you to die from light injuries");
            
            var badInjury = new Injured("Decapitated Head", you, 100, InjuryRegion.Head,w.InjurySystems.First(i=>i.IsDefault));
            you.Adjectives.Add(badInjury);

            Assert.IsFalse(you.Dead,"Expected death check to be at the end of the round");

            w.RunRound(new FixedChoiceUI(),you.GetFinalActions().OfType<LoadGunsAction>().Single());

            Assert.IsTrue(you.Dead,"Expected you to die at the end of the round");

        }
    }
}

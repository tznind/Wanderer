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
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;
using Tests.Actions;
using Enumerable = System.Linq.Enumerable;

namespace Tests.Systems
{
    class InjurySystemTests
    {
        [Test]
        public void Test_AllInjuriesUnique()
        {
            List<string> seenSoFar = new List<string>();

            var sys = new InjurySystem();

            HashSet<IAdjective> adjectives = new HashSet<IAdjective>();

            var a = Mock.Of<IActor>(b=>b.Adjectives == adjectives);

            for (int i = 0; i < 60; i++)
            {
                adjectives.Clear();
                sys.Apply(new SystemArgs(M.UI_GetChoice(new object()),i,null,a,Guid.Empty));

                //it should have applied 1 injury
                Assert.AreEqual(1,adjectives.Count);
                var injury = (Injured)adjectives.Single();

                Assert.AreNotEqual(InjuryRegion.None,injury);

                var newInjury = injury.Name;
                Assert.IsFalse(seenSoFar.Contains(newInjury),$"Saw duplicate injury '{newInjury}' for i={i}");
                seenSoFar.Add(newInjury);
            }
        }

        [Test]
        public void Test_LightInjuriesHealOverTime()
        {
            HashSet<IAdjective> adjectives = new HashSet<IAdjective>();

            var a = Mock.Of<IActor>(b=>b.Adjectives == adjectives
                                       && b.GetFinalBehaviours() == b.Adjectives.SelectMany(a=>a.GetFinalBehaviours(b))
                                       && b.BaseStats == new StatsCollection());

            //give them an injury
            var injury = new Injured("Bruised Shin", a, 1, InjuryRegion.Leg);
            a.Adjectives.Add(injury);
            
            for (int i = 0; i < 10; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(M.UI_GetChoice(typeof(object)), new LoadGunsAction(), a, a.GetFinalBehaviours());

                //after 9 round you should still be injured                
                if(i <9)
                    Assert.Contains(injury,a.Adjectives.ToArray());
                else
                    //on 10th round it should be gone
                    Assert.IsFalse(a.Adjectives.Contains(injury));
            }
        }
        [Test]
        public void Test_HeavyInjuriesGetWorseOverTime()
        {
            HashSet<IAdjective> adjectives = new HashSet<IAdjective>();

            var a = Mock.Of<IActor>(b=>b.Adjectives == adjectives
                                       && b.GetFinalBehaviours() == b.Adjectives.SelectMany(a=>a.GetFinalBehaviours(b))
                                       && b.BaseStats == new StatsCollection());

            //give them an injury
            var injury = new Injured("Cut Lip", a, 2, InjuryRegion.Leg);
            a.Adjectives.Add(injury);
            
            for (int i = 0; i < 10; i++)
            {
                var stack = new ActionStack();
                stack.RunStack(M.UI_GetChoice(typeof(object)), new LoadGunsAction(), a, a.GetFinalBehaviours());

                //after 2 rounds (0 and 1) you should still be injured                
                if(i == 0 )
                    StringAssert.AreEqualIgnoringCase("Cut Lip",injury.Name);
                if (i == 1)
                {
                    StringAssert.AreEqualIgnoringCase("Infected Cut Lip",injury.Name);
                    Assert.AreEqual(3,injury.Severity);
                    Assert.Contains(injury,a.Adjectives.ToArray());
                }

                //2 + 3 + 4 + 5 + 5
                if (i == 20)
                {
                    StringAssert.AreEqualIgnoringCase("Infected Cut Lip",injury.Name);
                    Assert.AreEqual(7,injury.Severity);
                }

                
                //2 + 3 + 4 + 5 + 6
                if (i == 21)
                {
                    StringAssert.AreEqualIgnoringCase("Infected Cut Lip",injury.Name);
                    Assert.AreEqual(8,injury.Severity);
                }
            }
        }

        [Test]
        public void Test_HealingAnInjury()
        {
            HashSet<IAdjective> adjectives = new HashSet<IAdjective>();

            var you = new You("You", new Room("someRoom", new World()));

            //you are a medic
            you.Adjectives.Add(new Medic(you));

            //you cannot heal even though you are a medic
            Assert.IsFalse(you.GetFinalActions().OfType<HealAction>().Any());

            //until you have good Savvy
            you.BaseStats[Stat.Savvy] = 50;
            
            //now you can heal stuff
            Assert.IsTrue(you.GetFinalActions().OfType<HealAction>().Any());

            //give them an injury
            var injury = new Injured("Cut Lip", you, 2, InjuryRegion.Leg);
            you.Adjectives.Add(injury);

            var stack = new ActionStack();

            Assert.Contains(injury,you.Adjectives.ToArray());
            stack.RunStack(new GetChoiceTestUI(you,injury), new HealAction(), you, you.GetFinalBehaviours());
            Assert.IsFalse(you.Adjectives.Contains(injury));
        

        }
    }
}

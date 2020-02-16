using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Plans;
using Wanderer.Systems;

namespace Tests.Plans
{
    class PlanTests : UnitTest
    {
        [Test]
        public void Test_EatWhenHungry()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            var ifHungryEatPlan = new Plan()
            {
                DoFrame = new FrameSourceCode(
                    @"new Frame((IActor)Recipient,((IActor)Recipient).GetFinalActions().OfType<EatAction>().FirstOrDefault(),0)"),
                Condition =
                {
                    new ConditionCode<SystemArgs>("Recipient.Adjectives.OfType<IInjured>().Any(a=>a.InjurySystem is HungerInjurySystem)")
                }
            };

            world.PlanningSystem.Plans.Add(ifHungryEatPlan);
            
            //let a round pass
            world.RunRound(GetUI(),new LoadGunsAction());

            //they should have no plans
            Assert.IsNull(((Npc)them).Plan);
            //they should not be hungry
            Assert.AreEqual(0,them.Adjectives.OfType<IInjured>().Count());
            
            //make them hungry
            new HungerInjurySystem().Apply(
                new SystemArgs(world,GetUI(),10/*TODO: again this is x10 vs Severity!*/,null,them,Guid.Empty)
                );
            
            //they should now be hungry
            Assert.AreEqual(1,them.Adjectives.OfType<IInjured>().Count());

            //let a round pass
            world.RunRound(GetUI(),new LoadGunsAction());

            //they cannot eat yet so will still be hungry with no plan
            Assert.IsNull(((Npc)them).Plan);
            Assert.AreEqual(1,them.Adjectives.OfType<IInjured>().Count());

            //give them something to eat (magically! normally this would be on an item)
            them.BaseActions.Add(new EatAction());
            
            //let a round pass
            world.RunRound(GetUI(),new LoadGunsAction());

            //they should no longer be hungry
            Assert.AreEqual(0,them.Adjectives.OfType<IInjured>().Count());
            Assert.IsNotNull(((Npc)them).Plan);

        }
    }
}

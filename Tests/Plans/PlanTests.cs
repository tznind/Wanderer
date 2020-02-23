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
                Do = new FrameSourceCode(
                    @"return Frame(Recipient,EatAction(),0)"),
                Condition =
                {
                    new ConditionCode<SystemArgs>("return Recipient:Has(Guid('89c18233-5250-4445-8799-faa9a888fb7f'))")
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

        [Test]
        public void Test_DeserializePlan()
        {

            string yaml = @"
- Name: Eat if hungry
  Condition:
    - Recipient.Has(new Guid(""89c18233-5250-4445-8799-faa9a888fb7f""))
  Do: new Frame(Recipient,GetFinalActions().OfType<EatAction>().FirstOrDefault(),0)
  ";

            var plans = Compiler.Instance.Deserializer.Deserialize<Plan[]>(yaml);

            Assert.AreEqual(1,plans.Length);
            Assert.AreEqual("Eat if hungry",plans[0].Name);

        }

        [Test]
        public void Test_FollowPlan()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            //move out of their room
            Assert.AreEqual(you.CurrentLocation,them.CurrentLocation);
            world.RunRound(GetUI(Direction.North),new LeaveAction());
            Assert.AreNotEqual(you.CurrentLocation,them.CurrentLocation);

            //give them a plan to follow you!
            world.PlanningSystem.Plans.Add(new FollowPlan(world.Player));

            //kill some time
            var ui = GetUI();
            world.RunRound(ui,new LoadGunsAction());
            
            //they should follow you
            Assert.AreEqual(you.CurrentLocation,them.CurrentLocation);
            Assert.IsInstanceOf(typeof(LeaveFrame),((Npc)them).Plan);
        }
    }
}

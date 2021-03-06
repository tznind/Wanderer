﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
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
                    @"return Frame(Recipient,FirstOrDefault(Recipient:Get('Eat')),0)"),
                Condition =
                {
                    new ConditionCode("return Recipient:Has(Guid('89c18233-5250-4445-8799-faa9a888fb7f'))")
                }
            };

            world.PlanningSystem.Plans.Add(ifHungryEatPlan);
            
            //let a round pass
            world.RunRound(GetUI(),new DoNothingAction(you));

            //they should have no plans
            Assert.IsNull(((Npc)them).Plan);
            //they should not be hungry
            Assert.AreEqual(0,them.Adjectives.OfType<IInjured>().Count());
            
            var hungerSystem = world.InjurySystems.Single(i=>i.Identifier == new Guid("89c18233-5250-4445-8799-faa9a888fb7f"));

            //make them hungry
            hungerSystem.Apply(
                new SystemArgs(world,GetUI(),10/*TODO: again this is x10 vs Severity!*/,null,them,Guid.Empty)
                );
            
            //they should now be hungry
            Assert.AreEqual(1,them.Adjectives.OfType<IInjured>().Count());

            //let a round pass
            world.RunRound(GetUI(),new DoNothingAction(you));

            //they cannot eat yet so will still be hungry with no plan
            Assert.IsNull(((Npc)them).Plan);
            Assert.AreEqual(1,them.Adjectives.OfType<IInjured>().Count());

            //they can eat themselves!
            world.ActionFactory.Create(world, them, "Eat");
            
            //let a round pass
            world.RunRound(GetUI(),new DoNothingAction(you));

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
    - Lua: true
  Do: 
    Lua: return nil
  ";

            var plans = Compiler.Instance.Deserializer.Deserialize<PlanBlueprint[]>(yaml);

            Assert.AreEqual(1,plans.Length);
            Assert.AreEqual("Eat if hungry",plans[0].Name);

        }

        [Test]
        public void Test_FollowPlan()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);

            //move out of their room
            Assert.AreEqual(you.CurrentLocation,them.CurrentLocation);
            world.RunRound(GetUI(Direction.North),new LeaveAction(you));
            Assert.AreNotEqual(you.CurrentLocation,them.CurrentLocation);

            //make sure they have the leave action so that they can
            //follow you
            them.BaseActions.Add(new LeaveAction(you));
            
            //give them a plan to follow you!
            world.PlanningSystem.Plans.Add(new FollowPlan(world.Player));

            //kill some time
            var ui = GetUI();
            world.RunRound(ui,new DoNothingAction(you));
            
            //they should follow you
            Assert.AreEqual(you.CurrentLocation,them.CurrentLocation);
            Assert.IsInstanceOf(typeof(LeaveFrame),((Npc)them).Plan);
        }

        [Test]
        public void TestDoFrame()
        {
            var code = new FrameSourceCode(@"return EquipmentFrame(
           Recipient,
           FirstOrDefault(Recipient:Get('Equipment')),
           EquipmentActionToPerform.PutOn,
           nil
        )");

            var you = YouInARoom(out IWorld world);

            var args = new SystemArgs(world,GetUI(),0,null,you,Guid.Empty);

            using(var lua = code.Factory.Create(world,args))
                Assert.IsInstanceOf(typeof(IAction),lua.DoString("return FirstOrDefault(Recipient:Get('Equipment'))")[0]);

            using(var lua = code.Factory.Create(world,args))
                Assert.IsInstanceOf(typeof(EquipmentActionToPerform),lua.DoString("return EquipmentActionToPerform.PutOn")[0]);

            using(var lua = code.Factory.Create(world,args))
                Assert.IsNull(lua.DoString("return GetFirstEquippableItem(Recipient)")[0]);
    
            Assert.IsNotNull(code.GetFrame(args));

        }
    }
}

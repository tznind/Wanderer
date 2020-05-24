using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tests.Actions;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests.CompilerTests
{
    class HasConditionTests : UnitTest
    {

        private SystemArgs GetSystemArgs(IActor forActor)
        {
            return new SystemArgs(forActor.CurrentLocation.World,null,0,forActor,forActor,Guid.Empty);
        }

        [Test]
        public void TestHasCondition_Actor()
        {
            YouInARoom(out IWorld world);

            var condition = new HasCondition("fff",SystemArgsTarget.Aggressor);

            Assert.IsFalse(condition.IsMet(world,GetSystemArgs(world.Player)));

            world.Player.BaseActions.Add(new TestAction(world.Player)
            {
                Name = "fff"
            });

            Assert.IsTrue(condition.IsMet(world,GetSystemArgs(world.Player)));
        }

        [Test]
        public void TestHasCondition_SystemArgs()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var condition = new HasCondition("fff",SystemArgsTarget.Aggressor);

            //nobody meets the condition
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,you,them,Guid.Empty)));
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,them,Guid.Empty)));

            //you now meet condition
            you.BaseActions.Add(new TestAction(world.Player)
            {
                Name = "fff"
            });

            // When args include both an Aggressor and a Recipient then the condition favors the Aggressor
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,them,you,Guid.Empty)));
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,you,them,Guid.Empty)));

            // When args only have a Recipient then the condition tests the Recipient
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,them,Guid.Empty)));
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,you,Guid.Empty)));
        }

        [Test]
        public void TestHasCondition_InvertLogic_SystemArgs()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var condition = new HasCondition("fff",SystemArgsTarget.Aggressor){InvertLogic = true};

            //everyone meets condition
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,you,them,Guid.Empty)));
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,them,Guid.Empty)));

            //you are now disqualified
            you.BaseActions.Add(new TestAction(world.Player)
            {
                Name = "fff"
            });

            // When args include both an Aggressor and a Recipient then the condition favors the Aggressor
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,them,you,Guid.Empty)));
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,you,them,Guid.Empty)));

            // When args only have a Recipient then the condition tests the Recipient
            Assert.IsTrue(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,them,Guid.Empty)));
            Assert.IsFalse(condition.IsMet(world,new SystemArgs(world, new FixedChoiceUI(),0,null,you,Guid.Empty)));
        }
    }
}

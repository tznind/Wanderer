﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tests.Actions;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Tests.CompilerTests
{
    class HasConditionTests : UnitTest
    {
        [Test]
        public void TestHasCondition_Actor()
        {
            YouInARoom(out IWorld world);

            var condition = new HasCondition<IActor>("fff");

            Assert.IsFalse(condition.IsMet(world,world.Player));

            world.Player.BaseActions.Add(new TestAction(world.Player)
            {
                Name = "fff"
            });

            Assert.IsTrue(condition.IsMet(world,world.Player));
        }

        [Test]
        public void TestHasCondition_SystemArgs()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var condition = new HasCondition<SystemArgs>("fff");

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
        public void TestHasCondition_UnknownType()
        {
            InARoom(out IWorld world);

            var condition = new HasCondition<bool>("fff");
            var ex = Assert.Throws<NotSupportedException>(() => condition.IsMet(world, true));

            Assert.AreEqual("Unknown T type System.Boolean",ex.Message);
        }

        [Test]
        public void TestHasCondition_Null()
        {
            InARoom(out IWorld world);

            var condition = new HasCondition<bool?>("fff");
            Assert.Throws<ArgumentNullException>(() => condition.IsMet(world, null));
        }
    }
}
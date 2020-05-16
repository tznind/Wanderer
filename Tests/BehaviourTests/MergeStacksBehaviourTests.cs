using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Stats;

namespace Tests.BehaviourTests
{
    public class MergeStacksBehaviourTests : UnitTest
    {
        [Test]
        public void TestMerging_IdenticalStacks()
        {
            var you = YouInARoom(out IWorld world);

            you.Items.Add(new ItemStack("toy cars", 1));
            you.Items.Add(new ItemStack("toy cars", 3));
            Assert.AreEqual(2,you.Items.Count);

            world.RunRound(GetUI(),new DoNothingAction(you));

            Assert.AreEqual(1,you.Items.Count);
            Assert.AreEqual(4,((IItemStack)you.Items.Single()).StackSize);
        }

        
        [Test]
        public void TestMerging_StacksWithActions()
        {
            YouInARoom(out IWorld world);

            Guid g = Guid.NewGuid();

            var master = new ItemBlueprint()
            {
                Name = "Laser Clip",
                Stack = 5,
                Identifier = g,
                MandatoryAdjectives = new []{"SingleUse"},
                Actions = new List<ActionBlueprint>{new ActionBlueprint(){Type = "FightAction"}},
                Stats = new StatsCollection(20)
            };

            var ref2 = new ItemBlueprint()
            {
                Ref = g.ToString()
            };


            var ref1 = new ItemBlueprint()
            {
                Ref = g.ToString()
            };

            world.ItemFactory.Blueprints.Add(master);
            var item1 = world.ItemFactory.Create(world, ref1);
            var item2 = world.ItemFactory.Create(world, ref2);

            Assert.IsTrue(item1.AreIdentical(item2));
        }

        [Test]
        public void TestMerging_DoNotMergeNonIdenticalStacks()
        {
            var you = YouInARoom(out IWorld world);

            you.Items.Add(new ItemStack("toy cars", 1));

            var b = new ItemStack("toy cars", 3);
            b.BaseStats[Stat.Fight] = 5;

            you.Items.Add(b);
            Assert.AreEqual(2,you.Items.Count);

            world.RunRound(GetUI(),new DoNothingAction(you));

            Assert.AreEqual(2,you.Items.Count);
            Assert.AreEqual(1,((IItemStack)you.Items.First()).StackSize);
            Assert.AreEqual(3,((IItemStack)you.Items.Skip(1).First()).StackSize);
        }
    }
}
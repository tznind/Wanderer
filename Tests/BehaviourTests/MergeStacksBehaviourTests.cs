using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
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

            world.RunRound(GetUI(),new LoadGunsAction());

            Assert.AreEqual(1,you.Items.Count);
            Assert.AreEqual(4,((IItemStack)you.Items.Single()).StackSize);
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

            world.RunRound(GetUI(),new LoadGunsAction());

            Assert.AreEqual(2,you.Items.Count);
            Assert.AreEqual(1,((IItemStack)you.Items.First()).StackSize);
            Assert.AreEqual(3,((IItemStack)you.Items.Skip(1).First()).StackSize);
        }
    }
}
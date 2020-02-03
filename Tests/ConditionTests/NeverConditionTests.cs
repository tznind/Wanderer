using System.Linq;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;
using StarshipWanderer.Factories;
using StarshipWanderer.Stats;

namespace Tests.ConditionTests
{
    public class NeverConditionTests
    {
        [Test]
        public void TestNever_IsMet_False()
        {
            var c = new NeverCondition<IActor>();
            Assert.IsFalse(c.IsMet(Mock.Of<IActor>()));
        }
        [Test]
        public void Test_NeverCondition_WithExpiry()
        {
            var yaml = 
@"
- Name: Sunglasses
  Stats:
    Savvy: 10
  Require:
    - Never()";  //<- sunglasses are about to be in fashion but not yet

            
            var itemFactory = new YamlItemFactory(yaml,new AdjectiveFactory());
            var item = itemFactory.Create(new World(), itemFactory.Blueprints.Single());

            Assert.AreEqual(0,item.GetFinalStats(Mock.Of<IActor>())[Stat.Savvy]);
            item.Require.Clear();
            Assert.AreEqual(10,item.GetFinalStats(Mock.Of<IActor>())[Stat.Savvy]);

        }

    }
}
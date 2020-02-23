using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Stats;

namespace Tests.ConditionTests
{
    public class NeverConditionTests : UnitTest
    {
        [Test]
        public void Test_NeverCondition_WithExpiry()
        {
            var yaml = 
@"
- Name: Sunglasses
  Stats:
    Savvy: 10
  Require:
    - return false";  //<- sunglasses are about to be in fashion but not yet

            
            var itemFactory = new YamlItemFactory(yaml,new AdjectiveFactory());
            var item = itemFactory.Create(new World(), itemFactory.Blueprints.Single());

            var you = YouInARoom(out _);
            you.BaseStats[Stat.Savvy] = 0;

            Assert.AreEqual(0,item.GetFinalStats(you)[Stat.Savvy]);
            item.Require.Clear();
            Assert.AreEqual(10,item.GetFinalStats(you)[Stat.Savvy]);

        }

    }
}
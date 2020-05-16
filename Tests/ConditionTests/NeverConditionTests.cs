using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
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

            
            var itemFactory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)};
            var w = new World();
            w.AllStats.GetOrAdd("Savvy");
            
            var item = itemFactory.Create(w, itemFactory.Blueprints.Single());

            var you = YouInARoom(out _);
            you.BaseStats["Savvy"] = 0;

            Assert.AreEqual(0,item.GetFinalStats(you)["Savvy"]);
            item.Require.Clear();
            Assert.AreEqual(10,item.GetFinalStats(you)["Savvy"]);

        }

    }
}
using System;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Factories;

namespace Tests.Actors
{
    class YamlItemFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingItem_FromBlueprint()
        {
            var adj = new AdjectiveFactory();

            //   var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "./Resources/Factions/Guncrew/Actors.yaml");

            var yaml = @"
- Name: Crumpled Pamphlet
  Dialogue: e088ff6e-60de-4a59-a9d8-b9406a2aed7c
- Name: Torn Pamphlet
  Dialogue: f1909b20-80c3-4af4-b098-b6bf22bf5ca8
";

            var factory = new YamlItemFactory(yaml, adj);
            Assert.AreEqual(2,factory.Blueprints.Length);

            var you = YouInARoom(out IWorld w);
            var item = factory.Create(factory.Blueprints[1]);

            Assert.AreEqual("Torn Pamphlet",item.Name);
            Assert.AreEqual(1,item.BaseActions.OfType<ReadAction>().Count());
            Assert.AreEqual(new Guid("f1909b20-80c3-4af4-b098-b6bf22bf5ca8"), item.NextDialogue);

        }
    }
}
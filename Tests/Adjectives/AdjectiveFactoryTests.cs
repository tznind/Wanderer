using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Factories;
using StarshipWanderer.Factories.Blueprints;

namespace Tests.Adjectives
{
    class AdjectiveFactoryTests
    {
        [TestCase("Strong")]
        [TestCase("Tough")]
        [TestCase("Rusty")]
        [TestCase("Dark")]
        public void TestCreate_AdjectiveBlueprint(string typeName)
        {
            var adj = new AdjectiveFactory();
            Assert.IsNotNull(adj.Create(Mock.Of<IHasStats>(),new AdjectiveBlueprint(){Type = typeName}));
        }
    }
}
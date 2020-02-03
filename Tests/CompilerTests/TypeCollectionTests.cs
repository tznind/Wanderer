using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Compilation;

namespace Tests.CompilerTests
{
    class TypeCollectionTests
    {
        [Test]
        public void TestTypeCollection_Factory()
        {
            var factory = new TypeCollectionFactory(typeof(IWorld).Assembly);

            var adjectives = factory.Create<IAdjective>();

            //no interfaces
            Assert.IsFalse(adjectives.Contains(typeof(IAdjective)));
            Assert.Contains(typeof(Strong),adjectives);

            var actors = factory.Create<IActor>(true,true);

            //no interfaces
            Assert.Contains(typeof(IActor),actors);
            Assert.Contains(typeof(Actor),actors);
            Assert.Contains(typeof(You),actors);
            Assert.Contains(typeof(Npc),actors);

        }
    }
}

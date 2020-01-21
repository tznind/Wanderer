using System.IO;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Factories;
using StarshipWanderer.Items;

namespace Tests.Actors
{
    class YamlActorFactoryTests : UnitTest
    {
        [Test]
        public void TestFactionActorsYaml()
        {
            var adj = new AdjectiveFactory();

         //   var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "./Resources/Factions/Guncrew/Actors.yaml");

         var yaml = @"
- Name: Centipede
  Adjectives:
    - Type: Giant
    - Type: Rusty
    - Type: Strong
    - Type: Tough
- Name: Crab
  Adjectives:
    - Type: Strong
";
         
            var actorFactory = new YamlActorFactory(yaml, null,new ItemFactory(adj), adj);
            Assert.GreaterOrEqual(actorFactory.Blueprints.Length , 2);

            var room = InARoom(out IWorld w);

            var actor = actorFactory.Create(w,room,actorFactory.Blueprints[1]);

            Assert.AreEqual("Crab",actor.Name);
            Assert.AreEqual("Strong",actor.Adjectives.Single().Name);    
        }
    }
}
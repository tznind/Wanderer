using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Factories;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

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
  Stats:
    Fight: 30
- Name: Crab
  Dialogue: 
    - 566ae926-a1fe-4209-9a15-fce026dbc5d1
  Adjectives:
    - Type: Strong
  Stats:
    Fight: 40
";
         
            var actorFactory = new YamlActorFactory(yaml, null,new ItemFactory(adj), adj);
            Assert.GreaterOrEqual(actorFactory.Blueprints.Length , 2);

            var room = InARoom(out IWorld w);

            var actor = actorFactory.Create(w,room,actorFactory.Blueprints[1]);

            Assert.AreEqual("Crab",actor.Name);
            Assert.AreEqual("Strong",actor.Adjectives.Single().Name);    
            
            Assert.AreEqual(40,actor.BaseStats[Stat.Fight]);    

            Assert.AreEqual(new Guid("566ae926-a1fe-4209-9a15-fce026dbc5d1"),actor.NextDialogue );
        }

        
    }
}
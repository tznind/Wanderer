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

         var yaml = @"
- Name: Centipede
  OptionalAdjectives:
    - Type: Giant
    - Type: Rusty
    - Type: Strong
    - Type: Tough
  Stats:
    Fight: 30
- Name: Crab
  Dialogue: 
    Verb: talk
    Next: 566ae926-a1fe-4209-9a15-fce026dbc5d1
  OptionalAdjectives:
    - Type: Strong
  Stats:
    Fight: 40
";
         
            var actorFactory = new YamlActorFactory(yaml, null,new ItemFactory(adj), adj);
            Assert.GreaterOrEqual(actorFactory.Blueprints.Length , 2);

            var room = InARoom(out IWorld w);

            var actor = actorFactory.Create(w,room,null,actorFactory.Blueprints[1],null);

            Assert.AreEqual("Crab",actor.Name);
            Assert.AreEqual("Strong",actor.Adjectives.Single().Name);    
            
            Assert.AreEqual(40,actor.BaseStats[Stat.Fight]);    

            Assert.AreEqual(new Guid("566ae926-a1fe-4209-9a15-fce026dbc5d1"),actor.Dialogue.Next );
        }


        [Test]
        public void TestCreatingActorWithItem_FromBlueprint()
        {

            string yaml = @"- Name: Servitor
  MandatoryAdjectives:
    - Type: Rusty
    - Type: Strong
    - Type: Tough
  Stats:
    Fight: 30
    Loyalty: 20
  MandatoryItems:
    - Name: Chronometer
      Stats:
        Value: 10";

            var room = InARoom(out IWorld w);
            var adj = new AdjectiveFactory();
            var actorFactory = new YamlActorFactory(yaml, null,new ItemFactory(adj), adj);
            var servitor = actorFactory.Create(w, room, null,actorFactory.Blueprints.Single(),null);

            Assert.AreEqual("Servitor",servitor.Name);
            Assert.Contains("Rusty",servitor.Adjectives.Select(a=>a.Name).ToArray());
            Assert.Contains("Strong",servitor.Adjectives.Select(a=>a.Name).ToArray());
            Assert.Contains("Tough",servitor.Adjectives.Select(a=>a.Name).ToArray());

            Assert.AreEqual(10,servitor.Items.Single().BaseStats[Stat.Value]);
            Assert.AreEqual("Chronometer",servitor.Items.Single().Name);
        }
    }
}
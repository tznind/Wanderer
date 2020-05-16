using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Stats;

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
    - Giant
    - Rusty
    - Strong
    - Tough
  Stats:
    Fight: 30
- Name: Crab
  Identifier: 24a1c3ad-fcdf-4c00-acf7-627e7f70c181
  Dialogue: 
    Verb: talk
    Next: 566ae926-a1fe-4209-9a15-fce026dbc5d1
  OptionalAdjectives:
    - Strong
  Stats:
    Fight: 40
";
         
            var actorFactory = new ActorFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(yaml)};
            Assert.GreaterOrEqual(actorFactory.Blueprints.Count , 2);

            var room = InARoom(out IWorld w);

            var actor = actorFactory.Create(w,room,null,actorFactory.Blueprints[1],null);

            Assert.AreEqual("Crab",actor.Name);
            Assert.AreEqual("Strong",actor.Adjectives.Single().Name);    
            
            Assert.AreEqual(40,actor.BaseStats[Stat.Fight]);    

            Assert.AreEqual(new Guid("566ae926-a1fe-4209-9a15-fce026dbc5d1"),actor.Dialogue.Next );
            Assert.AreEqual(new Guid("24a1c3ad-fcdf-4c00-acf7-627e7f70c181"),actor.Identifier);
        }


        [Test]
        public void TestCreatingActorWithItem_FromBlueprint()
        {

            string yaml = @"- Name: Servitor
  MandatoryAdjectives:
    - Rusty
    - Strong
    - Tough
  Stats:
    Fight: 30
    Loyalty: 20
  MandatoryItems:
    - Name: Chronometer
      Stats:
        Value: 10";

            var room = InARoom(out IWorld w);
            
            w.AllStats.GetOrAdd("Loyalty");

            var actorFactory = new ActorFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(yaml)};
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
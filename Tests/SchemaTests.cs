using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NUnit.Framework;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Tests
{
    class SchemaTests
    {
        readonly JsonSchemaGenerator _generator = 
            new JsonSchemaGenerator(new JsonSchemaGeneratorSettings()
            {
                FlattenInheritanceHierarchy = true,
                GenerateEnumMappingDescription = true,
                TypeMappers =
                {
                    new ObjectTypeMapper(typeof(StatsCollection),StatsDictionary)
                }
            });

        [Test]
        public void ActionBlueprintSchema()
        {
            CheckSchema<List<ActionBlueprint>>("actions.schema.json");
        }

        [Test]
        public void BehaviourBlueprintSchema()
        {
            CheckSchema<List<BehaviourBlueprint>>("behaviours.schema.json");
        }

        [Test]
        public void AdjectivesBlueprintSchema()
        {
            CheckSchema<List<AdjectiveBlueprint>>("adjectives.schema.json");
        }

        [Test]
        public void ItemBlueprintSchema()
        {
            CheckSchema<List<ItemBlueprint>>("items.schema.json");
        }
        [Test]
        public void ActorBlueprintSchema()
        {
            CheckSchema<List<ActorBlueprint>>("actors.schema.json");
        }
        
        [Test]
        public void RoomBlueprintSchema()
        {
            CheckSchema<List<RoomBlueprint>>("rooms.schema.json");
        }
        
        [Test]
        public void FactionBlueprintSchema()
        {
            CheckSchema<Faction>("faction.schema.json");
        }
        [Test]
        public void DialogueSchema()
        {
            CheckSchema<List<DialogueNodeBlueprint>>("dialogue.schema.json");
        }

        [Test]
        public void InjurySystemSchema()
        {
            CheckSchema<InjurySystem>("injury.schema.json");
        }

        [Test]
        public void PlanBlueprintSchema()
        {
            CheckSchema<List<PlanBlueprint>>("plans.schema.json");
        }

        [Test]
        public void SlotsSchema()
        {
            CheckSchema<SlotCollection>("slots.schema.json");
        }

        private void CheckSchema<T>(string filename)
        {
            var f = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources/Schemas", filename);
            FileAssert.Exists(f);

            var schema = _generator.Generate(typeof(T));
            string schemaJson = schema.ToJson();
            
            var fnew = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources/NewSchemas", filename));
            
            fnew.Directory.Create();
            File.WriteAllText(fnew.FullName,schemaJson);

            var newSchema = schemaJson.Replace("\r\n","\n");
            var oldSchema = File.ReadAllText(f).Replace("\r\n","\n");
            Assert.IsTrue(JToken.DeepEquals(newSchema,oldSchema),
                        $"schema is out of date for '{ filename }'.  New schema generated in {fnew.Directory.FullName}");
        }
        private static JsonSchema StatsDictionary(JsonSchemaGenerator arg1, JsonSchemaResolver arg2)
        {
            return arg1.Generate(typeof(Dictionary<Stat, double>));
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NJsonSchema.Generation;
using Wanderer.Dialogues;
using Wanderer.Systems;

namespace Tests
{
    class SchemaTests
    {
        [Test]
        public void DialogueSchema()
        {
            JsonSchemaGenerator generator = new JsonSchemaGenerator(new JsonSchemaGeneratorSettings());
            var schema = generator.Generate(typeof(List<DialogueNode>));
            TestContext.Out.WriteLine(schema.ToJson());
        }

        [Test]
        public void InjurySystemSchema()
        {
            JsonSchemaGenerator generator = new JsonSchemaGenerator(new JsonSchemaGeneratorSettings());
            var schema = generator.Generate(typeof(InjurySystem));
            TestContext.Out.WriteLine(schema.ToJson());
        }
    }
}

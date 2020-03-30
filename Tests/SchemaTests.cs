using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NJsonSchema.Generation;
using Wanderer.Dialogues;

namespace Tests
{
    class SchemaTests
    {
        [Test]
        public void Test()
        {
            
            JsonSchemaGenerator generator = new JsonSchemaGenerator(new JsonSchemaGeneratorSettings());
            var schema = generator.Generate(typeof(List<DialogueNode>));
            
        }
    }
}

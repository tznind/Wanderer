using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Systems;

namespace Tests.DialoguesTests
{
    class DialogueConditionCollectionYamlTypeConverterTests
    {
        [Test]
        public void TestDeserializeDialogueCondition_Friends()
        {

        }
        [Test]
        public void TestSerializeDialogueCondition_Friends()
        {
            var node = new DialogueNode()
            {
                Body = "Hey friend",
                Conditions = new DialogueConditionCollection(
                    new RelationshipCondition(Comparison.GreaterThanOrEqual, 10),
                    new RelationshipCondition(Comparison.GreaterThanOrEqual, 5)
                )
            };

            var dialogueSystem = new YamlDialogueSystem()
                {AllDialogues = new List<DialogueNode>(new[] {node})};

            var yaml = dialogueSystem.Serialize();

            //make sure the yaml generated can be parsed back
            var newInstance = new YamlDialogueSystem(yaml);

            StringAssert.Contains("Relationship(GreaterThanOrEqual,10)",yaml);

            StringAssert.Contains("Relationship(GreaterThanOrEqual,5)", yaml);
        }
    }
}

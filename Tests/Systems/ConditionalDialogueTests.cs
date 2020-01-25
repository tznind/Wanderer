using System;
using System.Collections.Generic;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Dialogues.Conditions;

namespace Tests.Systems
{
    class ConditionalDialogueTests : UnitTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestCondition_AreFriends(bool friends)
        {
            TwoInARoomWithRelationship(10,false,out You you, out IActor them, out IWorld w);
            them.BaseActions.Clear();

            var g1 = new Guid("93d68a59-d0ef-4df7-97af-fa3db0840bad");
            var g2 = new Guid("077860e8-9d23-4f0f-9ca4-19b3425ca9c3");
            var n1 = new DialogueNode()
            {
                Identifier = g1,
                Body = "Hey I want to give you all the space bucks!",
                Conditions = new DialogueConditionCollection(new RelationshipCondition(Comparison.GreaterThanOrEqual,5))

            };

            var n2 = new DialogueNode()
            {
                Identifier = g2,
                Body = "I will hunt you down",
                Conditions = new DialogueConditionCollection(new RelationshipCondition(Comparison.LessThan,-5))
            };

            w.Dialogue.AllDialogues = new List<DialogueNode>(new []{n1,n2});
            them.Dialogue.Banter = new[] {g1,g2};
            
            for (int i = 0; i < 100; i++)
            {
                var ui = new FixedChoiceUI("talk:Chaos Sam");
                w.RunRound(ui,new DialogueAction());

                Assert.Contains("Hey I want to give you all the space bucks!",ui.MessagesShown);
            }

        }

    }
}
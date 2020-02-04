﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;
using YamlDotNet.Serialization;

namespace Tests.Systems
{
    class DialogueTests : UnitTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestDialogue_ChoicesAffectRelationships(bool pickFriendly)
        {
            TwoInARoom(out You you, out IActor them, out IWorld w);

            var g = Guid.NewGuid();

            var tree = new DialogueNode()
            {
                Identifier = g,
                Body = new []{new TextBlock("Hello World") }
            };
            var o1 = new DialogueOption()
            {
                Attitude = 10,
                Text = "Hello to you friend"
            };

            var o2 = new DialogueOption()
            {
                Attitude = -10,
                Text = "Go to hell"
            };

            tree.Options = new List<DialogueOption> {o1, o2};

            them.Dialogue.Verb = "talk";
            them.Dialogue.Next = tree.Identifier;
            w.Dialogue.AllDialogues.Add(tree);

            var yaml = new Serializer().Serialize(new DialogueNode[]{tree});
            TestContext.Out.Write(yaml);

            var ui = GetUI("talk:Chaos Sam",pickFriendly ? o1 : o2);

            w.RunRound(ui,new DialogueAction());

            var r = w.Relationships.OfType<PersonalRelationship>().Single(r => r.AppliesTo(them, you));

            Assert.AreEqual(pickFriendly ? 10 : -10,r.Attitude);

        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestBanterDialogueCondition_AreFriends(bool areFriends)
        {
            TwoInARoomWithRelationship(areFriends ? 10 : -10,false, out You you, out IActor them, out IWorld w);

            var friend = new DialogueNode()
            {
                Identifier = new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                Body = new TextBlock[]{new TextBlock("Hello Friend") },
                Require = new List<ICondition<SystemArgs>>()
                {
                    new RelationshipCondition(Comparison.GreaterThanOrEqual,5)
                }

            };
            var foe = new DialogueNode()
            {
                Identifier = new Guid("00d77067-da1c-4c34-96ee-8a74353e4839"),
                Body = new TextBlock[]{new TextBlock("Hello Foe") },
                Require = new List<ICondition<SystemArgs>>()
                {
                    new RelationshipCondition(Comparison.LessThan,-4)
                }
            };

            them.Dialogue.Verb = "talk";
            them.Dialogue.Banter = new[]
            {
                new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                new Guid("00d77067-da1c-4c34-96ee-8a74353e4839")
            };

            w.Dialogue.AllDialogues.Add(friend);
            w.Dialogue.AllDialogues.Add(foe);
            
            var ui = GetUI("talk:Chaos Sam");
            w.RunRound(ui,new DialogueAction());

            Assert.Contains(areFriends ? "Hello Friend" : "Hello Foe",ui.MessagesShown);

        }

        [Test]
        public void Test_SimpleSubstitution()
        {
            var you = YouInARoom(out IWorld w);
            you.Name = "Flash";
            var npc = new Npc("Space Crab",you.CurrentLocation);
            npc.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: 
    - Text: ""Greetings {aggressor} I am {this}""";
         
            var dlg = new YamlDialogueSystem(yaml);

            var ui = GetUI();
            dlg.Apply(new SystemArgs(ui,0,you,npc,Guid.Empty));
            Assert.Contains("Greetings Flash I am Space Crab",ui.MessagesShown);
            
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Substitutions(bool areFriends)
        {
            TwoInARoomWithRelationship(areFriends ? 10:-10,false,out You you, out IActor them,out IWorld w);
            
            them.Name = "Space Crab";
            them.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: 
    - Text: ""Screeeee (this creature seems {DescribeRelationship})""";
            
            var dlg = new YamlDialogueSystem(yaml);

            var ui = GetUI();
            dlg.Apply(new SystemArgs(ui,0,you,them,Guid.Empty));

            if(areFriends)
                Assert.Contains("Screeeee (this creature seems friendly)",ui.MessagesShown);
            else
                Assert.Contains("Screeeee (this creature seems hostile)",ui.MessagesShown);
        }
        
        [Test]
        public void TestConditionalDialogue()
        {
            string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: This room is
    - Text: Pitch Black
      Condition: 
        - ""!PlaceHas<Light>()""
    - Text: Dimly Illuminated by your light
      Condition: 
        - PlaceHas<Light>()";

            var light = (ICondition<SystemArgs>)new PlaceHas<Light>();
            var notlight = (ICondition<SystemArgs>)Not<object>.Decorate(new PlaceHas<Light>());

            var system = new YamlDialogueSystem(yaml);
            Assert.IsNotNull(system);

            //TODO: test this with ActorHas


        }

    }

    
}

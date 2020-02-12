using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;
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
                    new ConditionCode<SystemArgs>("((IActor)Recipient).AttitudeTo(AggressorIfAny) > 5")
                }

            };
            var foe = new DialogueNode()
            {
                Identifier = new Guid("00d77067-da1c-4c34-96ee-8a74353e4839"),
                Body = new TextBlock[]{new TextBlock("Hello Foe") },
                Require = new List<ICondition<SystemArgs>>()
                {
                    new ConditionCode<SystemArgs>("((IActor)Recipient).AttitudeTo(AggressorIfAny) < -4")
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
    - Text: ""Greetings {AggressorIfAny} I am {Recipient}""";
         
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
    - Text: Screeeee (this creature seems friendly)
      Condition: 
        - Relationship > 0
    - Text: Screeeee (this creature seems hostile)
      Condition: 
        - Relationship < 0
    - Text: Screeeee (this creature seems indifferent)
      Condition: 
        - Relationship == 0";
            
            var dlg = new YamlDialogueSystem(yaml);

            var ui = GetUI();
            dlg.Apply(new SystemArgs(ui,0,you,them,Guid.Empty));

            if(areFriends)
                Assert.Contains("Screeeee (this creature seems friendly)",ui.MessagesShown);
            else
                Assert.Contains("Screeeee (this creature seems hostile)",ui.MessagesShown);
        }
        
        [Test]
        public void TestConditionalDialogue_PlaceHasLight()
        {
            string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: This room is
    - Text: Pitch Black
      Condition: 
        - ""!Place.Has<Light>()""
    - Text: Dimly Illuminated
      Condition: 
        - Place.Has<Light>()";

            var system = new YamlDialogueSystem(yaml);
            Assert.IsNotNull(system);

            var ui = GetUI();

            var room = InARoom(out _);

            system.Run(new SystemArgs(ui,0,
                Mock.Of<IActor>(a=> a.CurrentLocation == room),room,
                Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("This room is Pitch Black",ui.MessagesShown);

            room.Adjectives.Add(new Light(room));

            system.Run(new SystemArgs(ui,0,
                Mock.Of<IActor>(a=> a.CurrentLocation == room),room,
                Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("This room is Dimly Illuminated",ui.MessagesShown);
        }
        
        [Test]
        public void TestConditionalDialogue_ActorHasStat()
        {
            string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: The denizens of this degenerate bar 
    - Text: make you nervous
      Condition: 
        - ""!(AggressorIfAny.GetFinalStats()[Stat.Corruption] > 5)""
    - Text: seem like your kind of people
      Condition: 
        - AggressorIfAny.GetFinalStats()[Stat.Corruption] > 5";

            var system = new YamlDialogueSystem(yaml);
            Assert.IsNotNull(system);

            var ui = GetUI();

            var you = YouInARoom(out _);
            you.BaseStats[Stat.Corruption] = 0;

            system.Run(new SystemArgs(ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("The denizens of this degenerate bar make you nervous",ui.MessagesShown);

            you.BaseStats[Stat.Corruption] = 10;
            
            system.Run(new SystemArgs(ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("The denizens of this degenerate bar seem like your kind of people",ui.MessagesShown);
        }

    }

    
}

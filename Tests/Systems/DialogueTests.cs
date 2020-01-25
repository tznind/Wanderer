using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
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
        public void TestDialogue_HelloWorld(bool pickFriendly)
        {
            var you = YouInARoom(out IWorld w);
            var sam = new Npc("Chaos Sam", you.CurrentLocation);

            var g = Guid.NewGuid();

            var tree = new DialogueNode()
            {
                Identifier = g,
                Body = "Hello World"
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

            sam.Dialogue.Verb = "talk";
            sam.Dialogue.Next = tree.Identifier;
            w.Dialogue.AllDialogues.Add(tree);

            var yaml = new Serializer().Serialize(new DialogueNode[]{tree});
            TestContext.Out.Write(yaml);

            var ui = GetUI("talk:Chaos Sam",pickFriendly ? o1 : o2);

            w.RunRound(ui,new DialogueAction());

            var r = w.Relationships.OfType<PersonalRelationship>().Single(r => r.AppliesTo(sam, you));

            Assert.AreEqual(pickFriendly ? 10 : -10,r.Attitude);

        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Banter(bool areFriends)
        {
            var you = YouInARoom(out IWorld w);
            var sam = new Npc("Chaos Sam", you.CurrentLocation);

            var friend = new DialogueNode()
            {
                Identifier = new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                Body = "Hello Friend",
                Suits = Banter.Friend
            };
            var foe = new DialogueNode()
            {
                Identifier = new Guid("00d77067-da1c-4c34-96ee-8a74353e4839"),
                Body = "Hello Foe",
                Suits = Banter.Foe
            };

            sam.Dialogue.Verb = "talk";
            sam.Dialogue.Banter = new[]
            {
                new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                new Guid("00d77067-da1c-4c34-96ee-8a74353e4839")
            };

            w.Dialogue.AllDialogues.Add(friend);
            w.Dialogue.AllDialogues.Add(foe);

            //how does sam feel about you? how will he respond
            w.Relationships.Add(new PersonalRelationship(sam,you){Attitude = areFriends ? 10 : -10});

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
  Body: ""Greetings {aggressor} I am {this}""";
         
            var dlg = new DialogueSystem(yaml);

            var ui = GetUI();
            dlg.Apply(new SystemArgs(ui,0,you,npc,Guid.Empty));
            Assert.Contains("Greetings Flash I am Space Crab",ui.MessagesShown);
            
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Substitutions(bool areFriends)
        {
            var you = YouInARoom(out IWorld w);
            var npc = new Npc("Space Crab",you.CurrentLocation);
            npc.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: ""Screeeee (this creature seems {DescribeRelationship})""";

            w.Relationships.Add(new PersonalRelationship(npc,you){Attitude = areFriends ? 10 : -10});

            var dlg = new DialogueSystem(yaml);

            var ui = GetUI();
            dlg.Apply(new SystemArgs(ui,0,you,npc,Guid.Empty));

            if(areFriends)
                Assert.Contains("Screeeee (this creature seems friendly)",ui.MessagesShown);
            else
                Assert.Contains("Screeeee (this creature seems hostile)",ui.MessagesShown);
        }


    }
}

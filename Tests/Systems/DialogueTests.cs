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

            sam.NextDialogue = tree.Identifier;
            w.Dialogue.AllDialogues.Add(tree);

            var yaml = new Serializer().Serialize(new DialogueNode[]{tree});
            TestContext.Out.Write(yaml);

            var ui = GetUI(sam,pickFriendly ? o1 : o2);

            w.RunRound(ui,new TalkAction());

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
                Body = "Hello Friend",
                Suits = Banter.Friend
            };
            var foe = new DialogueNode()
            {
                Body = "Hello Foe",
                Suits = Banter.Foe
            };

            w.Dialogue.AllDialogues.Add(friend);
            w.Dialogue.AllDialogues.Add(foe);

            //how does sam feel about you? how will he respond
            w.Relationships.Add(new PersonalRelationship(sam,you){Attitude = areFriends ? 10 : -10});

            var ui = GetUI(sam);
            w.RunRound(ui,new TalkAction());

            Assert.Contains(areFriends ? "Hello Friend" : "Hello Foe",ui.MessagesShown);

        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Substitutions(bool areFriends)
        {
            var you = YouInARoom(out IWorld w);
            var npc = new Npc("Space Crab",you.CurrentLocation);
            npc.NextDialogue = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

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

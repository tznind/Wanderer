using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
    }
}

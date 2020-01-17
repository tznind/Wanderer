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

            sam.NextDialog = tree.Identifier;
            w.Dialogue.AllDialogues.Add(tree);

            var ui = GetUI(sam,pickFriendly ? o1 : o2);

            w.RunRound(ui,new TalkAction());

            var r = w.Relationships.OfType<PersonalRelationship>().Single(r => r.AppliesTo(sam, you));

            Assert.AreEqual(pickFriendly ? 10 : -10,r.Attitude);

        }
    }
}

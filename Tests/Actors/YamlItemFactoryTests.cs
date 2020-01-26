using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Factories;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace Tests.Actors
{
    class YamlItemFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingItem_FromBlueprint()
        {
            var adj = new AdjectiveFactory();

            //   var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "./Resources/Factions/Guncrew/Actors.yaml");

            var yaml = @"
- Name: Crumpled Pamphlet
  Dialogue: 
    Verb: read
    Next: e088ff6e-60de-4a59-a9d8-b9406a2aed7c
- Name: Torn Pamphlet
  Dialogue: 
    Verb: read
    Next: f1909b20-80c3-4af4-b098-b6bf22bf5ca8
";

            var factory = new YamlItemFactory(yaml, adj);
            Assert.AreEqual(2,factory.Blueprints.Length);

            var you = YouInARoom(out IWorld w);
            var item = factory.Create(factory.Blueprints[1]);

            Assert.AreEqual("Torn Pamphlet",item.Name);
            Assert.AreEqual(1,item.BaseActions.OfType<DialogueAction>().Count());
            Assert.AreEqual(new Guid("f1909b20-80c3-4af4-b098-b6bf22bf5ca8"), item.Dialogue.Next);

            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("f1909b20-80c3-4af4-b098-b6bf22bf5ca8"),
                Body = "Welcome to the ship"
            });

            you.Items.Add(item);

            var ui = new FixedChoiceUI("read:Torn Pamphlet");
            w.RunRound(ui,you.GetFinalActions().OfType<DialogueAction>().First());

            Assert.Contains("Welcome to the ship",ui.MessagesShown);


        }

        /// <summary>
        /// Test creating an item that requires high Savvy to use
        /// </summary>
        [Test]
        public void TestRequiresSavvy_ItemRead()
        {
            var yaml = @"
- Name: Encrypted Manual
  Dialogue: 
    Next: e088ff6e-60de-4a59-a9d8-b9406a2aed7c
  Require: 
    - ActorStat(Savvy,GreaterThan,50)
";
            var you = YouInARoom(out IWorld w);

            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("e088ff6e-60de-4a59-a9d8-b9406a2aed7c"),
                Body = "The book is filled with magic secrets"
            });

            var itemFactory = new YamlItemFactory(yaml, new AdjectiveFactory());

            you.Items.Add(itemFactory.Create(itemFactory.Blueprints.Single()));
            var ui = GetUI("read:Encrypted Manual");

            w.RunRound(ui,new DialogueAction());

            Assert.Contains(@"Item requirements not met:Savvy GreaterThan 50",ui.MessagesShown);

            you.BaseStats[Stat.Savvy] = 51;

            ui = GetUI("read:Encrypted Manual");
            w.RunRound(ui,new DialogueAction());

            Assert.Contains("The book is filled with magic secrets",ui.MessagesShown);

        }
    }
}
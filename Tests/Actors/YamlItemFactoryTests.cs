using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Tests.Actors
{
    class YamlItemFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingItem_FromBlueprint()
        {
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

            var factory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)};
            Assert.AreEqual(2,factory.Blueprints.Count);

            var you = YouInARoom(out IWorld w);
            var item = factory.Create(w, factory.Blueprints[1]);

            Assert.AreEqual("Torn Pamphlet",item.Name);
            Assert.AreEqual(new Guid("f1909b20-80c3-4af4-b098-b6bf22bf5ca8"), item.Dialogue.Next);

            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("f1909b20-80c3-4af4-b098-b6bf22bf5ca8"),
                Body = new List<TextBlock>
                {
                    new TextBlock( "Welcome to the ship")
                }

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
    - return BaseStats[Stat.Savvy] > 50
";
            var you = YouInARoom(out IWorld w);

            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("e088ff6e-60de-4a59-a9d8-b9406a2aed7c"),
                Body = new List<TextBlock>
                {
                    new TextBlock("The book is filled with magic secrets") 
                }
            });

            var itemFactory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)};

            you.Items.Add(itemFactory.Create(w, itemFactory.Blueprints.Single()));
            var ui = GetUI("read:Encrypted Manual");

            w.RunRound(ui,new DialogueAction(you.Items.First()));

            Assert.Contains(@"Item requirements not met:return BaseStats[Stat.Savvy] > 50",ui.MessagesShown);

            you.BaseStats[Stat.Savvy] = 51;

            ui = GetUI("read:Encrypted Manual");
            w.RunRound(ui,new DialogueAction(you.Items.First()));

            Assert.Contains("The book is filled with magic secrets",ui.MessagesShown);
        }

        [Test]
        public void TestCreatingItem_Stack()
        {
            string yaml = @"
- Name: Chips
  Stack: 1
- Name: Chips
  Stack: 10
- Name: Silver Bell";

            InARoom(out IWorld w);
            var itemFactory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)}; 
            var item = itemFactory.Create(w, itemFactory.Blueprints[0]);

            Assert.IsInstanceOf<IItemStack>(item);
            Assert.AreEqual(1,((IItemStack)item).StackSize);

            var item2 = itemFactory.Create(w, itemFactory.Blueprints[1]);

            Assert.IsInstanceOf<IItemStack>(item2);
            Assert.AreEqual(10,((IItemStack)item2).StackSize);
            
            var item3 = itemFactory.Create(w, itemFactory.Blueprints[2]);
            
            Assert.IsNotInstanceOf<IItemStack>(item3);
        }
    }
}
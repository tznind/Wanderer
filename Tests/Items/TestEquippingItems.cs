using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Stats;

namespace Tests.Items
{
    class TestEquippingItems : UnitTest
    {
        [Test]
        public void TestGiantHammers_RequireHandsToUse()
        {
            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Fight] = 0;
            you.AvailableSlots.Add("Hand",2);

            var itemFactory = new ItemFactory();

            //you have 2 giant hammers
            var twoHanded = new ItemSlot("Hand", 2, InjuryRegion.Hand,InjuryRegion.Arm);

            IItem item1;
            IItem item2;

            you.Items.Add(item1 = new Item("Hammer")
                .With(world,world.AdjectiveFactory,"Giant")
                .With(twoHanded));

            you.Items.Add(item2 = new Item("Hammer")
                .With(world,world.AdjectiveFactory,"Giant")
                .With(twoHanded));

            Assert.AreEqual(0,you.GetFinalStats()[Stat.Fight],"Expected hammers to be of no help because they were not equipped");

            ActionStack s = new ActionStack();

            Assert.IsTrue(s.RunStack(world,new FixedChoiceUI(EquipmentActionToPerform.PutOn, item1), new EquipmentAction(item1), you, null));

            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight],"Expected hammer to be boosting your fight");

            //cannot equip it because it is already equipped
            Assert.IsFalse(s.RunStack(world,new FixedChoiceUI(EquipmentActionToPerform.PutOn, item2), new EquipmentAction(item2), you, null),"Expected attempt to wield 2 hammers to have failed");
            
            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight],"Expected you to still be wielding 1");

            //now you get 4 arms!
            you.AvailableSlots["Hand"] = 4;

            //now you can equip it!
            Assert.IsTrue(s.RunStack(world,new FixedChoiceUI(EquipmentActionToPerform.PutOn, item2), new EquipmentAction(item2), you, null),"Now you have 4 arms 2 hammers should be fine!");

            Assert.AreEqual(60,you.GetFinalStats()[Stat.Fight]);

            //take it off again
            Assert.IsTrue(s.RunStack(world,new FixedChoiceUI(EquipmentActionToPerform.TakeOff, item2), new EquipmentAction(item2), you, null));

            //stats should return to before value
            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight]);
        }
        [Test]
        public void TestRustyPistolInRustyRoom_EquipForSmallBonus()
        {
            var you = YouInARoom(out IWorld world);
            var room = you.CurrentLocation;
            room.Adjectives.Add(new Adjective(room)
            {
                Name = "Rusty",
                IsPrefix = true,
                StatsRatio = new StatsCollection(0.5)
            });

            you.BaseStats[Stat.Fight] = 20;
            you.AvailableSlots.Add("Hand",2);

            IItem item1 = new Item("Pistol")
                .With(Stat.Fight,20)
                .With(new ItemSlot("Hand",1,InjuryRegion.Hand));
           
             item1.Adjectives.Add(
                 new Adjective(item1)
                 {
                     Name = "Rusty",
                     IsPrefix = true,
                     StatsRatio = new StatsCollection(0.5)
                 });           
            
            you.Items.Add(item1);

            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Did not expect rusty room to be a problem");

            ActionStack s = new ActionStack();

            Assert.IsTrue(s.RunStack(world,new FixedChoiceUI(EquipmentActionToPerform.PutOn, item1), new EquipmentAction(item1), you, null));

            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight],"Expected hammer to be boosting your fight");
        }

        [Test]
        public void TestEquippingItem_TooMuchFight()
        {
                var yaml = @"
- Name: Suave Shirt
  Slot: 
    Name: Chest
    NumberRequired: 1
  Require: 
    - return BaseStats[Stat.Fight] <= 10
";

                var you = YouInARoom(out IWorld w);
                you.AvailableSlots.Add("Chest",1);

                var itemFactory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)};

                var shirt = itemFactory.Create(w, itemFactory.Blueprints.Single());

                you.Items.Add(shirt);
                var ui = GetUI(EquipmentActionToPerform.PutOn,shirt);

                w.RunRound(ui,new EquipmentAction(shirt));

                Assert.IsFalse(shirt.IsEquipped);

                Assert.Contains(@"Item requirements not met:return BaseStats[Stat.Fight] <= 10",ui.MessagesShown);

                you.BaseStats[Stat.Fight] = 10;
            
                ui = GetUI(EquipmentActionToPerform.PutOn,shirt);

                w.RunRound(ui,new EquipmentAction(shirt));
                Assert.IsTrue(shirt.IsEquipped);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.ActorOnly;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Factories;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace Tests.Items
{
    class TestEquippingItems : UnitTest
    {
        [Test]
        public void TestGiantHammers_RequireHandsToUse()
        {
            var you = YouInARoom(out IWorld w);
            you.BaseStats[Stat.Fight] = 0;
            you.AvailableSlots.Add("Hand",2);

            var itemFactory = new ItemFactory(new AdjectiveFactory());

            //you have 2 giant hammers
            var twoHanded = new ItemSlot("Hand", 2, InjuryRegion.Hand,InjuryRegion.Arm);

            IItem item1;
            IItem item2;

            you.Items.Add(item1 = itemFactory.Create<Giant>("Hammer").With(twoHanded));
            you.Items.Add(item2 = itemFactory.Create<Giant>("Hammer").With(twoHanded));

            Assert.AreEqual(0,you.GetFinalStats()[Stat.Fight],"Expected hammers to be of no help because they were not equipped");

            ActionStack s = new ActionStack();

            Assert.IsTrue(s.RunStack(new FixedChoiceUI(EquipmentActionToPerform.PutOn, item1), new EquipmentAction(), you, null));

            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight],"Expected hammer to be boosting your fight");

            //cannot equip it because it is already equipped
            Assert.IsFalse(s.RunStack(new FixedChoiceUI(EquipmentActionToPerform.PutOn, item2), new EquipmentAction(), you, null),"Expected attempt to wield 2 hammers to have failed");
            
            Assert.AreEqual(30,you.GetFinalStats()[Stat.Fight],"Expected you to still be wielding 1");

            //now you get 4 arms!
            you.AvailableSlots["Hand"] = 4;

            //now you can equip it!
            Assert.IsTrue(s.RunStack(new FixedChoiceUI(EquipmentActionToPerform.PutOn, item2), new EquipmentAction(), you, null),"Now you have 4 arms 2 hammers should be fine!");

            Assert.AreEqual(60,you.GetFinalStats()[Stat.Fight]);

        }
        [Test]
        public void TestRustyPistolInRustyRoom_EquipForSmallBonus()
        {
            var you = YouInARoom(out IWorld w);
            var room = you.CurrentLocation;
            room.Adjectives.Add(new Rusty(room));

            you.BaseStats[Stat.Fight] = 20;
            you.AvailableSlots.Add("Hand",2);

            var itemFactory = new ItemFactory(new AdjectiveFactory());
            IItem item1 = itemFactory.Create<Rusty>("Pistol").With(Stat.Fight,20).With(new ItemSlot("Hand",1,InjuryRegion.Hand));
            you.Items.Add(item1);

            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Did not expect rusty room to be a problem");

            ActionStack s = new ActionStack();

            Assert.IsTrue(s.RunStack(new FixedChoiceUI(EquipmentActionToPerform.PutOn, item1), new EquipmentAction(), you, null));

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
    - BaseStats[Stat.Fight] <= 10
";

                var you = YouInARoom(out IWorld w);
                you.AvailableSlots.Add("Chest",1);

                var itemFactory = new YamlItemFactory(yaml, new AdjectiveFactory());

                var shirt = itemFactory.Create(w, itemFactory.Blueprints.Single());

                you.Items.Add(shirt);
                var ui = GetUI(EquipmentActionToPerform.PutOn,shirt);

                w.RunRound(ui,new EquipmentAction());

                Assert.IsFalse(shirt.IsEquipped);

                Assert.Contains(@"Item requirements not met:BaseStats[Stat.Fight] <= 10",ui.MessagesShown);

                you.BaseStats[Stat.Fight] = 10;
            
                ui = GetUI(EquipmentActionToPerform.PutOn,shirt);

                w.RunRound(ui,new EquipmentAction());
                Assert.IsTrue(shirt.IsEquipped);
        }
    }
}

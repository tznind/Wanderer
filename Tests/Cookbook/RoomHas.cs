using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Wanderer;
using Wanderer.Stats;

namespace Tests.Cookbook
{
    class RoomHas : Recipe
    {
        private string adjectives = @"
- Name: Medical
";

        private string slots = @"
Arm: 2
";

        private string rooms = @"
- Name: Warehouse
  FixedLocation: 0,0,0
  MandatoryItems:
    - Name: Bionic Arm
      Stats:
        Fight: 20
      Slot:
        Name: Arm
        NumberRequired: 1
      EquipRequire:
         - Check: Room
           Has: Medical

- Name: Med Bay
  FixedLocation: -1,0,0
  MandatoryAdjectives:
    - Medical
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = base.Setup("defaultslots.yaml",slots,"adjectives.yaml", adjectives,"rooms.yaml",rooms);

            Assert.IsEmpty(world.Player.Items);
            world.Player.BaseStats[Stat.Fight] = 0;

            RunRound(world,"PickUp","Bionic Arm");

            Assert.AreEqual("Bionic Arm",world.Player.Items.Single().Name);

            Assert.AreEqual(0,world.Player.GetFinalStats()[Stat.Fight]);

            RunRound(world,"Equipment [Wanderer]","PutOn","Bionic Arm");
            
            Assert.IsFalse(world.Player.Items.Single().IsEquipped);

            //go to the medical bay
            GoWest(world);
            
            Assert.AreEqual("Med Bay",world.Player.CurrentLocation.Name);
            
            Assert.AreEqual(0,world.Player.GetFinalStats()[Stat.Fight]);

            RunRound(world,"Equipment [Wanderer]","PutOn","Bionic Arm");
            
            Assert.IsTrue(world.Player.Items.Single().IsEquipped);
            Assert.AreEqual(20,world.Player.GetFinalStats()[Stat.Fight]);
            
            GoEast(world);
            Assert.AreEqual(20,world.Player.GetFinalStats()[Stat.Fight]);
        }
    }
}

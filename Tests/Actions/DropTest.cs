using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Items;

namespace Tests.Actions
{
    class DropTest : UnitTest
    {
        [Test]
        public void TestDropSomething()
        {
            var you = YouInARoom(out IWorld w);
            var hot = new Item("Something Hot")
            {
                Slot = new ItemSlot("Hand",2)
            };

            you.Items.Add(hot);
            hot.IsEquipped = true;

            Assert.Contains(hot,you.Items.ToArray());
            Assert.IsEmpty(you.CurrentLocation.Items);
            
            w.RunRound(GetUI(hot),new DropAction());

            Assert.IsEmpty(you.Items);
            Assert.Contains(hot,you.CurrentLocation.Items.ToArray());

            Assert.IsFalse(hot.IsEquipped);
            
        }
    }
}

using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;
using Tests.Actions;

namespace Tests.Items
{
    class ItemTests
    {
        [Test]
        public void Test_DarkRoom()
        {
            var world = new World();
            var darkRoom = new Room("Dark Room", world);
            darkRoom.Adjectives.Add(new Dark(darkRoom));

            var globe = new Item("Glo Globe");
            globe.Adjectives.Add(new Light(globe));

            darkRoom.Items.Add(globe);

            var you = new You("Wanderer",darkRoom);
            you.BaseStats[Stat.Fight] = 20;

            Assert.AreEqual(10,you.GetFinalStats()[Stat.Fight],"Expect fight to be at penalty due to room being dark");
            Assert.AreEqual(0,you.Items.Count);
            Assert.AreEqual(1,you.CurrentLocation.Items.Count);

            var stack = new ActionStack();
            stack.RunStack(new GetChoiceTestUI(globe), new PickUpAction(), you,new IBehaviour[0]);

            Assert.AreEqual(1,you.Items.Count);
            Assert.AreEqual(0,you.CurrentLocation.Items.Count);
            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Expect fight to be back to normal now that you ignore dark");

            darkRoom.Adjectives.Clear();

            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Expect fight to stay 20 because lights don't help when it's not dark");
            
            you.Items.Clear();
            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Now that it is light the torch doesn't matter");

        }
    }

}

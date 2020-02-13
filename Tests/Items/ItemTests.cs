using System;
using System.Linq;
using Moq;
using NLog.Fluent;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Behaviours;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Places;
using Wanderer.Stats;
using Tests.Actions;

namespace Tests.Items
{
    class ItemTests : UnitTest
    {
        [Test]
        public void Test_DarkRoom()
        {
            var world = new World();
            var darkRoom = new Room("Dark Room", world,'-');
            darkRoom.Adjectives.Add(new Dark(darkRoom));

            Assert.IsTrue(darkRoom.Has<Dark>());

            world.Map.Add(new Point3(0,0,0),darkRoom);

            var globe = new Item("Glo Globe");
            globe.Adjectives.Add(new Light(globe));

            darkRoom.Items.Add(globe);

            var you = new You("Wanderer",darkRoom);
            you.BaseStats[Stat.Fight] = 20;

            Assert.AreEqual(10,you.GetFinalStats()[Stat.Fight],"Expect fight to be at penalty due to room being dark");
            Assert.AreEqual(0,you.Items.Count);
            Assert.AreEqual(1,you.CurrentLocation.Items.Count);

            var stack = new ActionStack();
            stack.RunStack(world,new FixedChoiceUI(globe), new PickUpAction(), you,new IBehaviour[0]);

            Assert.AreEqual(1,you.Items.Count);
            Assert.AreEqual(0,you.CurrentLocation.Items.Count);
            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Expect fight to be back to normal now that you ignore dark");

            //2+ light globes shouldn't boost your fight above the baseline
            var globe2 = new Item("Glo Globe");
            globe2.Adjectives.Add(new Light(globe));

            you.Items.Add(globe2);

            Assert.AreEqual(2,you.Items.Count);
            Assert.AreEqual(0,you.CurrentLocation.Items.Count);
            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Expect fight to be back to normal now since multiple globes shouldn't stack");



            darkRoom.Adjectives.Clear();

            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Expect fight to stay 20 because lights don't help when it's not dark");
            
            you.Items.Clear();
            Assert.AreEqual(20,you.GetFinalStats()[Stat.Fight],"Now that it is light the torch doesn't matter");

        }
        
        [Test]
        public void Test_DeadActorDropsItems()
        {
            var world = new World();
            var room = new Room("SomeRoom", world,'-');
            var npc = new Npc("Handyman", room);
            world.Map.Add(new Point3(0,0,0),room);

            var teddy = new Item("Teddy");
            npc.Items.Add(teddy);

            world.Population.Add(npc);

            Assert.Contains(npc,room.Actors.ToArray());
            Assert.IsEmpty(room.Items);

            npc.Kill(GetUI(), Guid.Empty, "nukes");

            //your gone but stay in room just dead
            Assert.IsTrue(world.Population.Contains(npc));
            Assert.IsTrue(npc.Dead);
            Assert.AreEqual("Dead Handyman",npc.ToString());

            //but teddy lives on, to be picked up by someone else
            Assert.Contains(teddy, room.Items.ToArray(),"Expected teddy to be in the room after the npc died");

            Assert.IsEmpty(npc.Items);
        }
        
        [Test]
        public void Test_RustyMakesNegativeModifiersWorse()
        {
            var itemFactory = new ItemFactory(new AdjectiveFactory());

            var you = new You("Dave", new Room("SomeRoom",new World(), 'f'));
            
            //This mask makes you less convincing.  When its rusty it should be even less convincing!
            Assert.Less(
                itemFactory.Create<Rusty>("Mask").With(Stat.Coerce,-10).GetFinalStats(you)[Stat.Coerce],
                new Item("Mask").With(Stat.Coerce,-10).GetFinalStats(you)[Stat.Coerce]);
        }
    }

}

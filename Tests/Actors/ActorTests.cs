using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace Tests.Actors
{
    class ActorTests : UnitTest
    {
        [Test]
        public void TestAddOccupant()
        {
            var you = YouInARoom(out IWorld world);
            world.Population.Add(you);

            var room1 = world.Player.CurrentLocation;

            Assert.Contains(you,world.Population.ToArray());

            var frank = new Npc("Frank",room1);

            
            Assert.AreEqual(room1,you.CurrentLocation);
            Assert.AreEqual(room1,frank.CurrentLocation);

            Assert.Contains(you,world.Population.ToArray());
            Assert.Contains(frank,world.Population.ToArray());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestDecideActor_ConsidersRelationships(bool areFriends)
        {
            var you = YouInARoom(out IWorld w);
            var them = new Npc("Dave", you.CurrentLocation);

            w.Relationships.Add(new PersonalRelationship(them, you) {Attitude = areFriends ? 10 : -10});

            //Do you want to pick player for something unkind?
            Assert.That(()=>them.DecideActor(new[] {you}, -5), areFriends? (IResolveConstraint) Is.Empty : Contains.Item(you));
            
            //You wouldn't wish this action on your worst enemy!
            Assert.That(()=>them.DecideActor(new[] {you}, -500),Is.Empty);

            //Do you want to pick player for something nice?
            Assert.That(()=>them.DecideActor(new[] {you}, 5), areFriends? (IResolveConstraint) Contains.Item(you):Is.Empty);
            
            //You wouldn't want to give him your house though
            Assert.That(()=>them.DecideActor(new[] {you}, 500),Is.Empty);
        }


        [Test]
        public void TestDeadPeople_DoNotFeelEmotions()
        {
            var room = InARoom(out IWorld w);
            var dave = new Npc("Tragic Dave", room);

            var cares = new RelationshipFormingBehaviour(dave);
            dave.BaseBehaviours.Add(cares);

            //dave cares
            Assert.Contains(cares,dave.GetFinalBehaviours().ToArray());

            dave.Kill(new FixedChoiceUI(),Guid.Empty,"Nukes");
            
            Assert.Contains(cares,dave.BaseBehaviours.ToArray(),"Expected behaviour to still be there in case he is resurrected");
            Assert.IsFalse(dave.GetFinalBehaviours().Contains(cares), "But expected it not to be manifest in the world");

            //boom you are back to life!
            dave.Dead = false;

            Assert.Contains(cares,dave.GetFinalBehaviours().ToArray(),"Expected behaviour be back on again now you are alive");
        }

        [Test]
        public void TestActors_AreIdentical()
        {
            var room = InARoom(out IWorld w);

            var n1 = new Npc("A", room);
            var n2 = new Npc("A", room);
            
            Assert.IsFalse(n1.AreIdentical(null),"Other is null");

            Assert.IsTrue(n1.AreIdentical(n1));
            Assert.IsTrue(n1.AreIdentical(n2));

            var item = new Item("A");
            Assert.IsFalse(n1.AreIdentical(item),"Types differ");

            n2.Name = "Troll";
            Assert.IsFalse(n1.AreIdentical(n2),"Names differ");

            n1.Name = "Troll";
            Assert.IsTrue(n1.AreIdentical(n2),"Names are same again");

            n1.BaseStats[Stat.Fight] = 500;
            Assert.IsFalse(n1.AreIdentical(n2),"One fights better");

            n2.BaseStats[Stat.Fight] = 500;
            Assert.IsTrue(n1.AreIdentical(n2),"Both fights well");


        }
    }
}

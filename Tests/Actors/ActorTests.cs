using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Relationships;

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
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using Tests.Actions;

namespace Tests.Relationships
{
    class RelationshipTests
    {
        [Test]
        public void Test_RelationshipEndsOnDeath()
        {
            IWorld world = new World();
            var room = new Room("Test Room", world,'-');

            var bob = new Npc("Bob", room);
            var frank = new Npc("Frank", room);
            var dave = new Npc("Dave", room);

            world.Relationships.Add(new Relationship(bob, frank)); //goes
            world.Relationships.Add(new Relationship(frank, bob)); //goes
            world.Relationships.Add(new Relationship(dave, frank)); //stays

            Assert.AreEqual(3,world.Relationships.Count);

            //when bob dies
            bob.Kill(new GetChoiceTestUI(new object()),Guid.Empty);

            //his relationship to frank should not exist
            Assert.AreEqual(1, world.Relationships.Count);
        }
    }
}

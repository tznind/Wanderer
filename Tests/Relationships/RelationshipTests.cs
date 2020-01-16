using System;
using System.Collections.Generic;
using System.Linq;
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
    class RelationshipTests : UnitTest
    {
        [Test]
        public void Test_RelationshipEndsOnDeath()
        {
            IWorld world = new World();
            var room = new Room("Test Room", world,'-');

            var bob = new Npc("Bob", room);
            var frank = new Npc("Frank", room);
            var dave = new Npc("Dave", room);

            world.Relationships.Add(new PersonalRelationship(bob, frank)); //goes
            world.Relationships.Add(new PersonalRelationship(frank, bob)); //goes
            world.Relationships.Add(new PersonalRelationship(dave, frank)); //stays

            Assert.AreEqual(3,world.Relationships.Count);

            //when bob dies
            bob.Kill(GetUI(),Guid.Empty);

            //his relationship to frank should not exist
            Assert.AreEqual(1, world.Relationships.Count);
        }

        [Test]
        public void Test_NpcDontFightFriends()
        {
            IWorld world = new World();
            var room = new Room("Test Room", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var you = new You("wanderer", room);
            var bob = new Npc("Bob", room); 
            bob.BaseActions.Clear();
            bob.BaseActions.Add(new FightAction());
            
            //the only thing bob does is fight
            Assert.AreEqual(1,bob.BaseActions.Count);

            world.Relationships.Add(new PersonalRelationship(bob, you){Attitude = 500});

            var ui = GetUI();
            world.RunRound(ui, new LoadGunsAction());

            Assert.IsNull(ui.Log.RoundResults.FirstOrDefault(r => r.Message.Contains("fought")),"Did not expect bob to fight you because they have a good relationship");
        }

        [Test]
        public void Test_FightingMakesYouEnemies()
        {
            IWorld world = new World();
            var room = new Room("Test Room", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var you = new You("wanderer", room);
            var bob = new Npc("Bob", room); 
            
            //don't do anything bob like wander off!
            bob.BaseActions.Clear();
            
            Assert.IsEmpty(world.Relationships.Where(r=>r.AppliesTo(bob,you) && r.Attitude <0),"bob should not have a negative opinion of you starting out");

            var ui = new FixedChoiceUI(bob,bob);

            //fight each other
            world.RunRound(ui, new FightAction());

            var youAndBob = world.Relationships.OfType<PersonalRelationship>()
                .SingleOrDefault(r => r.AppliesTo(bob, you) && r.Attitude < 0);
            
            Assert.IsNotNull(youAndBob,"bob should now be angry at you");
            
            var attitudeBefore = youAndBob.Attitude;

            //fight again
            world.RunRound(ui, new FightAction());

            Assert.Greater(attitudeBefore,youAndBob.Attitude,"Expected continuing to fight to make matters worse");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FightingIsFrownedUponByOthers(bool areFriends)
        {
            //when I'm in a room with 2 people
            IWorld world = new World();
            var room = new Room("Test Room", world,'-');
            world.Map.Add(new Point3(0,0,0), room);

            var you = new You("wanderer", room);
            var bob = new Npc("Bob", room); 
            var bobsFriend = new Npc("Bobs Friend", room); 
            
            //don't do anything bob like wander off!
            bob.BaseActions.Clear();
            bobsFriend.BaseActions.Clear();
            
            Assert.IsEmpty(world.Relationships.Where(r=>r.AppliesTo(bob,you) && r.Attitude <0),"bob should not have a negative opinion of you starting out");
            
            //if bobs friend likes him create a relationship
            if(areFriends)
                world.Relationships.Add(new PersonalRelationship(bobsFriend,bob){Attitude = 10});

            var ui = new FixedChoiceUI(bob);

            //fight each other
            world.RunRound(ui, new FightAction());

            var youAndBob = world.Relationships.OfType<PersonalRelationship>()
                .SingleOrDefault(r => r.AppliesTo(bob, you) && r.Attitude < 0);
            var youAndBobsFriend = world.Relationships.OfType<PersonalRelationship>()
                .SingleOrDefault(r => r.AppliesTo(bobsFriend, you) && r.Attitude < 0);
            
            Assert.IsNotNull(youAndBob,"bob should now be angry at you");

            if(areFriends)
                Assert.IsNotNull(youAndBobsFriend,"bobs friend should also be angry at you");
            else
                Assert.IsNull(youAndBobsFriend,"bobs friend should not care that you hit bob because they aren't really friends");
            

        }
    }
}

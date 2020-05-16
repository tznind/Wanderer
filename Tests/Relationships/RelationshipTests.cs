using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Tests.Actions;

namespace Tests.Relationships
{
    class RelationshipTests : UnitTest
    {
        [Test]
        public void Test_RelationshipEndsOnDeath()
        {
            var room = InARoom(out IWorld world);

            var bob = new Npc("Bob", room);
            var frank = new Npc("Frank", room);
            var dave = new Npc("Dave", room);

            world.Relationships.Add(new PersonalRelationship(bob, frank)); //goes
            world.Relationships.Add(new PersonalRelationship(frank, bob)); //goes
            world.Relationships.Add(new PersonalRelationship(dave, frank)); //stays

            Assert.AreEqual(3,world.Relationships.Count);

            //when bob dies
            bob.Kill(GetUI(),Guid.Empty, "space hamsters");

            //his relationship to frank should not exist
            Assert.AreEqual(1, world.Relationships.Count);
        }

        [Test]
        public void TestRelationshipToString()
        {
            var you = YouInARoom(out IWorld world);

            var bob = new Npc("Bob", you.CurrentLocation);
            var relationship = new PersonalRelationship(bob, you) {Attitude = 500};
            Assert.AreEqual("Bob to Test Wanderer of 500",relationship.ToString());
        }
        [Test]
        public void Test_NpcDontFightFriends()
        {
            TwoInARoom(out You you, out IActor them, out IWorld world);
            var room = you.CurrentLocation;
            them.BaseActions.Clear();
            them.BaseActions.Add(new FightAction(you));
            
            //the only thing bob does is fight
            Assert.AreEqual(1,them.BaseActions.Count);

            world.Relationships.Add(new PersonalRelationship(them, you){Attitude = 500});

            var ui = GetUI();
            world.RunRound(ui, new DoNothingAction(you));

            Assert.IsNull(ui.Log.RoundResults.FirstOrDefault(r => r.Message.Contains("fought")),"Did not expect bob to fight you because they have a good relationship");
        }

        [Test]
        public void Test_FightingMakesYouEnemies()
        {
            var you = YouInARoom(out IWorld world);
            var room = you.CurrentLocation;

            var bob = new Npc("Bob", room); 
            
            //don't do anything bob like wander off!
            bob.BaseActions.Clear();
            
            Assert.IsEmpty(world.Relationships.Where(r=>r.AppliesTo(bob,you) && r.Attitude <0),"bob should not have a negative opinion of you starting out");

            var ui = new FixedChoiceUI(bob,bob);

            //fight each other
            world.RunRound(ui, new FightAction(you));

            var youAndBob = world.Relationships.OfType<PersonalRelationship>()
                .SingleOrDefault(r => r.AppliesTo(bob, you) && r.Attitude < 0);
            
            Assert.IsNotNull(youAndBob,"bob should now be angry at you");
            
            var attitudeBefore = youAndBob.Attitude;

            //fight again
            world.RunRound(ui, new FightAction(you));

            Assert.Greater(attitudeBefore,youAndBob.Attitude,"Expected continuing to fight to make matters worse");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FightingIsFrownedUponByOthers(bool areFriends)
        {
            //when I'm in a room with 2 people
            var you = YouInARoom(out IWorld world);
            var room = you.CurrentLocation;

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
            world.RunRound(ui, new FightAction(you));

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
        [Test]
        public void TestFactionRelationships_BetweenFactions()
        {
            TwoInARoom(out You you,out IActor them,out IWorld world);

            var f1 = new Faction("F1",FactionRole.Civilian);
            var f2 = new Faction("F2",FactionRole.Civilian);

            you.FactionMembership.Add(f1);
            them.FactionMembership.Add(f2);
            them.BaseActions.Clear();
            
            Assert.IsEmpty(world.Relationships.OfType<FactionRelationship>().ToArray());

            var ui = new FixedChoiceUI(them);

            //fight each other
            world.RunRound(ui, new FightAction(you));

            //not only do you hate each other now but your factions should also hate each other
            var newRelationship = world.Relationships.OfType<FactionRelationship>().Single();
            Assert.IsInstanceOf<InterFactionRelationship>(newRelationship);

            Assert.Less(newRelationship.Attitude,0);

            // relationship should be from the fight victim towards the evil attacker!
            Assert.AreEqual(f2,newRelationship.HostFaction);
            Assert.AreEqual(f1,((InterFactionRelationship)newRelationship).ObservedFaction);


        }
    }
}

using System;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Tests.ConditionTests
{
    class HasGuidTest : UnitTest
    {

        [TestCase(true)]
        [TestCase(false)]
        public void TestHasGuid_Deserialization(bool useNot)
        {
            string yaml = @$"
Text: You have a jolly glo globe.
Condition:
  - return AggressorIfAny.CurrentLocation:Has(Guid('6fa349e4-aefe-4ebc-9922-e3476ea1dba7')) {(useNot?" == false":"")}";

            var block = Compiler.Instance.Deserializer.Deserialize<TextBlock>(yaml);


            var you = YouInARoom(out IWorld world);

            you.Identifier = new Guid("6fa349e4-aefe-4ebc-9922-e3476ea1dba7");

            Assert.AreEqual(!useNot,
                block.Condition.Single().IsMet(world, new SystemArgs(world,null, 0, you, null, Guid.Empty)));
        }

        [Test]
        public void TestHasWithFactions()
        {
            var room = InARoom(out IWorld w);

            var g = Guid.NewGuid();

            Assert.IsFalse(room.Has(g));
            
            var troll = new Npc("Troll",room);

            Assert.IsFalse(room.Has(g));

            troll.FactionMembership.Add(new Faction("Troll Kingdom",FactionRole.Civilian){
                Identifier = g
            });

            Assert.IsTrue(room.Has(g),"g is a Faction Identifier and troll is in that faction so room should Has the faction");
            Assert.IsTrue(troll.Has(g),"g is a Faction Identifier and troll is in that faction");

            Assert.IsTrue(room.Has("Troll Kingdom"));
            Assert.IsTrue(troll.Has("Troll Kingdom"));
        }
    }
}

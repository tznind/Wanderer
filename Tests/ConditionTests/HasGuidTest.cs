using System;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
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
    }
}

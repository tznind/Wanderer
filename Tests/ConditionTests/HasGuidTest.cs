using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Compilation;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Systems;
using Has = StarshipWanderer.Conditions.Has;

namespace Tests.ConditionTests
{
    class HasGuidTest : UnitTest
    {
        [Test]
        public void Test_GuidCondition()
        {
            var you = YouInARoom(out _);
            
            var g = Guid.NewGuid();
            var condition = new Has(g);

            Assert.IsFalse(condition.IsMet(you));
            Assert.IsFalse(condition.IsMet(you.CurrentLocation));

            you.Identifier = g;

            Assert.IsTrue(condition.IsMet(you));
            Assert.IsTrue(condition.IsMet(you.CurrentLocation));

        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestHasGuid_Deserialization(bool useNot)
        {
            string yaml = @$"
Text: You have a jolly glo globe.
Condition:
  - ""{(useNot?"!":"")}AggressorIfAny.CurrentLocation.Has(6fa349e4-aefe-4ebc-9922-e3476ea1dba7)""";

            var block = Compiler.Instance.Deserializer.Deserialize<TextBlock>(yaml);


            var you = YouInARoom(out IWorld _);

            you.Identifier = new Guid("6fa349e4-aefe-4ebc-9922-e3476ea1dba7");

            Assert.AreEqual(!useNot,
                block.Condition.Single().IsMet(new SystemArgs(null, 0, you, null, Guid.Empty)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Factories;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Tests.ConditionTests
{
    class ConditionSerializationTests : UnitTest
    {
        [TestCase("false")]
        [TestCase("true")]
        public void TestConstructors(string condition)
        {
            var yaml =
                @$"
- Name: MedKit
  Require: 
    - {condition}
";
            var itemFactory = new YamlItemFactory(yaml,new AdjectiveFactory());
            var createdInstance = itemFactory.Blueprints.Single().Require.Single();

            Assert.AreEqual(condition == "true", createdInstance.IsMet(Mock.Of<IActor>()));

        }
    }
}

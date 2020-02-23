using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Tests.ConditionTests
{
    class ConditionSerializationTests : UnitTest
    {
        [TestCase("return false")]
        [TestCase("return true")]
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

            Assert.AreEqual(condition == "return true", createdInstance.IsMet(new World(),Mock.Of<IActor>()));

        }
    }
}

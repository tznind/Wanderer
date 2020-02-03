using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;
using StarshipWanderer.Factories;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Tests.ConditionTests
{
    class ConditionSerializationTests : UnitTest
    {
        [TestCase("Never()",typeof(NeverCondition<IActor>))]
        [TestCase("Always()",typeof(AlwaysCondition<IActor>))]
        public void TestConstructors(string condition,Type expectedType)
        {
            var yaml =
                @$"
- Name: MedKit
  Require: 
    - {condition}
";
            var itemFactory = new YamlItemFactory(yaml,new AdjectiveFactory());
            var createdInstance = itemFactory.Blueprints.Single().Require.Single();
            Assert.IsInstanceOf(expectedType,createdInstance);
            Assert.AreEqual(condition,createdInstance.SerializeAsConstructorCall());

        }
    }
}

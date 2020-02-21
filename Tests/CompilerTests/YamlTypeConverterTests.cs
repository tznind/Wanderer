using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer.Actions;
using Wanderer.Behaviours;
using Wanderer.Compilation;

namespace Tests.CompilerTests
{
    class YamlTypeConverterTests
    {
        [Test]
        public void TestExistingClass_InstanceGenerated()
        {
            var converter = new YamlTypeConverter<IAction>();
            Assert.IsInstanceOf(typeof(LoadGunsAction),converter.ParseScalar("LoadGunsAction", typeof(IAction)));
        }
        [Test]
        public void TestBadClass_NonExistant()
        {
            var converter = new YamlTypeConverter<IAction>();
            var ex = Assert.Throws<ParseException>(()=>converter.ParseScalar("FFF", typeof(IAction)));

            StringAssert.Contains("Could not find Type 'FFF'",ex.Message);
        }
        [Test]
        public void TestBadClass_WrongType()
        {
            var converter = new YamlTypeConverter<IBehaviour>();
            var ex = Assert.Throws<ParseException>(()=>converter.ParseScalar("LoadGunsAction", typeof(IBehaviour)));

            StringAssert.Contains("Could not find Type 'LoadGunsAction' (either it does not exist or it is not a IBehaviour)",ex.Message);
        }
    }
}

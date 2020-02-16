using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer.Factories;
using Wanderer.Systems.Validation;

namespace Tests
{
    class ResourcesTutorialTests
    {
        [Test]
        public void TestEmpty()
        {
            var f = new WorldFactory()
            {
                ResourcesDirectory = TestContext.CurrentContext.WorkDirectory
            };

            f.Create();

            var v = new WorldValidator();
            v.Validate(f);
            Assert.IsEmpty(v.Errors.ToString());
            Assert.IsEmpty(v.Warnings.ToString());
        }
    }
}

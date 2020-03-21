using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var dir = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory, "EmptyDir"));

            if(dir.Exists)
                dir.Delete(true);

            dir.Create();

            var f = new WorldFactory()
            {
                ResourcesDirectory = dir.FullName
            };

            f.Create();

            var v = new WorldValidator();
            v.Validate(f);
            Assert.IsEmpty(v.Errors.ToString());
            Assert.IsEmpty(v.Warnings.ToString());
            Assert.AreEqual(0,v.ErrorCount);
            Assert.AreEqual(0,v.WarningCount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests
{
    class WorldFactoryTests
    {
        [Test]
        public void TestYaml_DialogueFiles()
        {
            var f = new WorldFactory(){
                ResourcesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,"Resources")
            };

            var world = f.Create();

            Assert.Greater(world.Dialogue.AllDialogues.Count,0);
        }

        [Test]
        public void TestYaml_ActorFiles()
        {
            var f = new WorldFactory(){
                ResourcesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,"Resources")
            };
            f.Create();
        }

        [Test]
        public void BadYaml_String()
        {
            var ex = Assert.Throws<ArgumentException>(()=>new YamlDialogueSystem("fffff"));
            StringAssert.Contains("Error in dialogue yaml",ex.Message);
        }

        [Test]
        public void BadYaml_File()
        {
            var fi = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory, "troll.yaml"));
            File.WriteAllText(fi.FullName,"ffff");

            var ex = Assert.Throws<ArgumentException>(()=>new YamlDialogueSystem(fi));
            StringAssert.Contains("Error in dialogue yaml",ex.Message);
            StringAssert.Contains("troll.yaml",ex.Message);
        }
    }
}

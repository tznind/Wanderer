using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Factories;
using StarshipWanderer.Systems;

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

            Assert.IsNotEmpty(f.GetAllDialogueYaml());

            var dialogue = new YamlDialogueSystem(f.GetAllDialogueYaml().ToArray());
            Assert.Greater(dialogue.AllDialogues.Count,0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Systems;

namespace Tests
{
    class WorldFactoryTests
    {
        [Test]
        public void TestYaml_DialogueFiles()
        {
            var f = new WorldFactory();
            
            Assert.IsNotEmpty(f.GetAllDialogueYaml());

            var dialogue = new DialogueSystem(f.GetAllDialogueYaml().ToArray());
            Assert.Greater(dialogue.AllDialogues.Count,0);
        }
    }
}

using System.IO;
using NUnit.Framework;
using Wanderer.Factories;

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
    }
}

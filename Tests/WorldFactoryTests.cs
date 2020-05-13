using System.IO;
using NUnit.Framework;
using Wanderer.Factories;

namespace Tests
{
    class WorldFactoryTests
    {
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

using NUnit.Framework;
using Wanderer;

namespace Tests.Cookbook
{
    class StartingRoom : Recipe
    {
        private string roomYaml =
@"
- Name: Somewhere Cool
  FixedLocation: 0,0,0
  Unique: true
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = Setup(roomYaml);
            Assert.AreEqual("Somewhere Cool",world.Player.CurrentLocation.Name);
            GoWest(world);
            Assert.AreEqual("Empty Room",world.Player.CurrentLocation.Name);
        }

    }
}
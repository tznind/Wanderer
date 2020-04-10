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
  Identifier: b1aa5ce4-213a-46b5-aa57-63831376b81d
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = SetupRoom(roomYaml);
            Assert.AreEqual("Somewhere Cool",world.Player.CurrentLocation.Name);
            GoWest(world);
            Assert.AreEqual("Empty Room",world.Player.CurrentLocation.Name);
        }

    }
}
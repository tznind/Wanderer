using NUnit.Framework;

namespace Tests.Cookbook
{
    class SameItemToManyRooms : Recipe
    {
        private string roomYaml = @"
- Name: Armoury
  MandatoryItems:
   - Base: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Base: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Base: e4ff5be4-233a-46b5-bb57-63831376b81d
";

        private string itemsYaml = @"
- Name: Rose
  Identifier: e4ff5be4-233a-46b5-bb57-63831376b81d
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = base.SetupRoom(roomYaml, itemsYaml);
            
            Assert.GreaterOrEqual(world.Player.CurrentLocation.Items.Count,3);
            Assert.IsTrue(world.Player.CurrentLocation.Items.TrueForAll(i=>i.Name == "Rose"));
        }
    }
}
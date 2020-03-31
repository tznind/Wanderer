using NUnit.Framework;

namespace Tests.Cookbook
{
    class EquippableWeapon : Recipe
    {
        
        private string slotsYaml = 
            @"
Wrist: 2
";

        private string itemYaml = @"
- Name: Wrist blade
  Stats:
    Fight: 10
  Slot:
   Name: Wrist
   NumberRequired: 1";

        [Test]
        public void ConfirmRecipe()
        {
            var world = base.SetupItem(slotsYaml, itemYaml);

            world.Player.SpawnItem("Wrist blade");

            Assert.IsTrue(world.Player.CanEquip(world.Player.Items[0],out _));
        }
    }
}
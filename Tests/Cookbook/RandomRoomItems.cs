using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tests.Cookbook
{
    class RandomRoomItems : Recipe
    {

        string itemsYaml = 
        @"
- Name: Rose
- Name: Egg
        ";

        string roomsYaml = 
        @"
- Name: Chamber of Horrors
        ";


        string roomsYaml2 = 
        @"
- Name: Chamber of Horrors
  OptionalItemsMax: 0
        ";

        [Test]
        public void ConfirmRecipe()
        {
            var world = SetupRoom(roomsYaml);
            Assert.AreEqual("Chamber of Horrors",world.Player.CurrentLocation.Name);
            Assert.AreEqual(0,world.Player.CurrentLocation.Items.Count);

            world = SetupRoom(roomsYaml,itemsYaml);
            Assert.GreaterOrEqual(world.Player.CurrentLocation.Items.Count,1);

            world = SetupRoom(roomsYaml2,itemsYaml);
            Assert.AreEqual(0,world.Player.CurrentLocation.Items.Count);
            
        }

    }
}
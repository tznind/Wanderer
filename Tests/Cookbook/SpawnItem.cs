using System.Linq;
using NUnit.Framework;

namespace Tests.Cookbook
{
    class SpawnItem : Recipe
    {
        string room = @"
- Name: Shop
  MandatoryActors:
    - Name: Goblin Shopkeeper
      # Don't let her wander off
      SkipDefaultActions: true
      Dialogue: 
        Verb: Shopping
        Next: 66e99df7-efd9-46cc-97a1-9fed851e0d8f
      MandatoryItems:
         - Name: Shiny Pebble";

        string dialogue = @"
- Identifier: 66e99df7-efd9-46cc-97a1-9fed851e0d8f
  Body:
    - Text: Here burk, want to buy this painted rock?
  Options:
     - Text: Yes please, heres 20 gold!
       Condition: 
          - Variable: Gold >= 20
       Effect:
          - Spawn: Shiny Pebble
          - Set: Gold -= 20
     - Text: Lend us some gold will you?
       SingleUse: true
       Effect:
          - Set: Gold += 20
     - Text: No thanks... my days of chasing shine are long behind me";

        [Test]
        public void ConfirmRecipe()
        {
            var world = Setup("rooms.yaml",room,"dialogue.yaml",dialogue);

            //player doesn't have 20 gold
            Assert.Throws<OptionNotAvailableException>(()=>RunRound(world,"Shopping:Goblin Shopkeeper","Yes please, heres 20 gold!"));

            Assert.AreEqual(0,world.Player.Items.Count);

            RunRound(world,"Shopping:Goblin Shopkeeper","Lend us some gold will you?");
            RunRound(world,"Shopping:Goblin Shopkeeper","Yes please, heres 20 gold!");

            Assert.AreEqual(1,world.Player.Items.Count);
            Assert.AreEqual("Shiny Pebble",world.Player.Items.Single().Name);
        }
        
    }
}

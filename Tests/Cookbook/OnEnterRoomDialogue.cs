using NUnit.Framework;
using Wanderer.Adjectives;

namespace Tests.Cookbook
{
    class OnEnterRoomDialogue : Recipe
    {
        string behaviour =
            @"
- Name: DialogueOnEnter
  Identifier: 5ae55edf-36d0-4878-bbfd-dbbb23d42b88
  OnEnter: 
   Condition: 
     - Lua: AggressorIfAny == World.Player
     - Lua: Room == Behaviour.Owner
     - Lua: Room.Dialogue.Next ~= nil
   Effect: 
     - Lua: World.Dialogue:Apply(SystemArgs(World,UserInterface,0,AggressorIfAny,Room,Round))
     - Lua: Room.Dialogue.Next = null
";

        string room =
            @"
- Name: Dank Cellar
  Behaviours:
    - Ref: 5ae55edf-36d0-4878-bbfd-dbbb23d42b88
  Dialogue:
    Next: 6da41741-dada-4a52-85d5-a019cd9d38f7
";

        string dialogue =
            @"
- Identifier: 6da41741-dada-4a52-85d5-a019cd9d38f7
  Body: 
   - Text: Goblins fill the room from floor to ceiling
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = Setup("dialogue.yaml", dialogue, "rooms.yaml", room, "behaviours.yaml", behaviour);
            GoWest(world, out FixedChoiceUI ui);

            Assert.Contains("Goblins fill the room from floor to ceiling",ui.MessagesShown);
            Assert.IsNull(world.Player.CurrentLocation.Dialogue.Next);

        }
    }
}
using System.Linq;
using NUnit.Framework;
using Wanderer.Actors;
using Wanderer.Items;

namespace Tests.Cookbook
{
    class Ammo : Recipe
    {

        string slotsYaml = 
        @"
Hand: 2
";

        string itemYaml = @"
- Name: Laser Clip
  Stack: 2
  MandatoryAdjectives:
    - SingleUse
  Actions: 
    - Type: FightAction
      Stats: 
         Fight: 20
  Require:
    - return this:Has('LaserPowered')

- Name: Laser Pistol
  Slot:
    Name: Hand
    NumberRequired: 1
  MandatoryAdjectives:
    - LaserPowered";

    string adjectivesYaml = 
    "- Name: LaserPowered";
        

    string injurySystemYaml = 
    @"
Identifier: 3bfc44ce-28ba-4fa8-951a-f97ec6dddf0f
Name: Laser Damage
IsDefault: true
FatalThreshold: 100
FatalVerb: injuries

Injuries:
- Name: Laser Burn
  Severity: 10";

        [Test]
        public void ConfirmRecipe()
        {
            var world = SetupItem(slotsYaml,itemYaml,adjectivesYaml,injurySystemYaml);

            var clip = (IItemStack)world.Player.SpawnItem("Laser Clip");
            Assert.IsFalse(clip.RequirementsMet(world.Player));

            //now you have the pistol
            var pistol = world.Player.SpawnItem("Laser Pistol");
            Assert.IsFalse(pistol.IsEquipped);
            Assert.IsFalse(clip.RequirementsMet(world.Player));

            world.Player.Equip(pistol);
            Assert.IsTrue(clip.RequirementsMet(world.Player));

            var dummy = new Npc("Target Dummy",world.Player.CurrentLocation);
            
            Assert.AreEqual(2,clip.StackSize);
            RunRound(world,"Fight [Laser Clip(2)]",dummy);
            Assert.AreEqual(1,clip.StackSize);
            RunRound(world,"Fight [Laser Clip(1)]",dummy);
            Assert.IsTrue(clip.IsErased);
            Assert.IsFalse(world.Player.Items.Contains(clip));
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Tests.Cookbook
{
    class Grenade : Recipe
    {
        
        string injurySystemYaml = 
            @"
Identifier: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
Name: Blast Damage
FatalThreshold: 100
FatalVerb: injuries

Injuries:
- Name: Wounded
  Severity: 10";

        private string itemYaml = @"
- Name: Grenade
  InjurySystem: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
  Stack: 1
  MandatoryAdjectives:
   - SingleUse
  Actions:
    - Type: FightAction
      Stats: 
         Fight: 30
      Effect:
        #Injury everyone in the room
        - World:GetSystem('7ccafc68-d51f-4408-861c-f1d7e4e6351a'):ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,20,AggressorIfAny,null,Round))
    ";

        [Test]
        public void ConfirmRecipe()
        {
            var world = base.Setup("items.yaml",itemYaml,"blast.injury.yaml",injurySystemYaml);

            var grenade = world.Player.SpawnItem("Grenade");

            var dummy = new Npc("Target Dummy",world.Player.CurrentLocation);
            
            Assert.IsTrue(world.Player.Items.Single().Name.Equals("Grenade"));
            RunRound(world,"Fight [Grenade(1)]",dummy);
            Assert.IsTrue(grenade.IsErased);
            Assert.IsEmpty(world.Player.Items,"Expected grenade to be used and now gone");

            //splash damage hit + initial hit
            Assert.AreEqual(2,dummy.Adjectives.OfType<IInjured>().Count());

            //splash damage hit to yourself
            Assert.AreEqual(1,world.Player.Adjectives.OfType<IInjured>().Count());
            
        }
    }
}

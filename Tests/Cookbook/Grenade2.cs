using System.Linq;
using NUnit.Framework;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Tests.Cookbook
{
    class Grenade2 : Recipe
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
        - Apply:
            Identifier: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
            #Name: Blast Damage <- You could also use this
            Intensity: 20
            All: true
        - Apply: 
            #Identifier: 8cfad1e6-39f9-4993-831f-57234290f558 <- You could also use this 
            Name: Relationship
            Intensity: -10
            All: true
    ";

        [Test]
        public void ConfirmRecipe()
        {
            var world = base.Setup("items.yaml",itemYaml,"blast.injury.yaml",injurySystemYaml);

            var grenade = world.Player.SpawnItem("Grenade");

            var dummy1 = new Npc("Target Dummy1",world.Player.CurrentLocation);
            var dummy2 = new Npc("Target Dummy2",world.Player.CurrentLocation);
            
            //Don't wander off or anything
            dummy1.BaseActions.Clear();
            dummy2.BaseActions.Clear();
            dummy1.BaseBehaviours.Clear();
            dummy2.BaseBehaviours.Clear();

            Assert.AreEqual(0,world.Relationships.SumBetween(dummy1,world.Player), "To start with nobody should be angry");
            Assert.AreEqual(0,world.Relationships.SumBetween(dummy2,world.Player), "To start with nobody should be angry");

            Assert.IsTrue(world.Player.Items.Single().Name.Equals("Grenade"));
            RunRound(world,"Fight [Grenade(1)]",dummy1);
            Assert.IsTrue(grenade.IsErased);
            Assert.IsEmpty(world.Player.Items,"Expected grenade to be used and now gone");

            //splash damage hit + initial hit
            Assert.AreEqual(2,dummy1.Adjectives.OfType<IInjured>().Count());
            Assert.AreEqual(1,dummy2.Adjectives.OfType<IInjured>().Count());

            //splash damage hit to yourself
            Assert.AreEqual(1,world.Player.Adjectives.OfType<IInjured>().Count());

            //everyone should be angry at you
            Assert.AreEqual(-10,world.Relationships.SumBetween(dummy1,world.Player));
            Assert.AreEqual(-10,world.Relationships.SumBetween(dummy2,world.Player));
        }
    }
}

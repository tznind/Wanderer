using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Cookbook
{
    class Factions : Recipe
    {
        private string factionyaml = @"
Identifier: 8a9d6962-ddd6-4209-9738-e9c7ef9b2a67
Name: Order of the Twisted Sigil
Role: Opposition
# Dark Yellow
Color: 6
DefaultSlots:
  Soul: 1
  Hand: 2
Forenames:
  - Ramirez
  - Thomas
  - France
Surnames: 
  - Cunningham
  - Smith  
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = Setup(
                "/Factions/Cult/cult.faction.yaml", factionyaml,
                "/1.actors.yaml", "- Name: Bob",
                "/Factions/Cult/2.actors.yaml", "- Name: Frank",
                "/Factions/Cult/3.actors.yaml", "- Name: Dave");
            
            Assert.AreEqual(1,world.Factions.Count);
            Assert.IsTrue(world.Factions.Single().DefaultSlots.ContainsKey("Soul"));
            Assert.IsTrue(world.Factions.Single().DefaultSlots.ContainsKey("Hand"));
            
            Assert.AreEqual(3,world.ActorFactory.Blueprints.Count);
            
            Assert.IsNull(world.ActorFactory.Blueprints.Single(b=>b.Name.Equals("Bob")).Faction);

            Assert.AreEqual(new Guid("8a9d6962-ddd6-4209-9738-e9c7ef9b2a67"), world.ActorFactory.Blueprints.Single(b=>b.Name.Equals("Frank")).Faction);
            Assert.AreEqual(new Guid("8a9d6962-ddd6-4209-9738-e9c7ef9b2a67"), world.ActorFactory.Blueprints.Single(b=>b.Name.Equals("Dave")).Faction);
        }
    }
}
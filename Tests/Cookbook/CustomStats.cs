using NUnit.Framework;
using Wanderer.Stats;

namespace Tests.Cookbook
{
    class CustomStats : Recipe
    {
        private string statsYaml = 
@"
- Sanity
- Seduction
";
        private string roomYaml =
@"
- Name: Relaxing Bar
  Stats:
    Seduction: 10
  MandatoryItems:
    - Name: Helmet of Madness
      Require:
        - Stat: Seduction <= 0
      Stats:
        Sanity: -500
";

        [Test]
        public void ConfirmRecipe()
        {
            Assert.AreEqual(new Stat("Trouble"),new Stat("Trouble"));

            // Create a new collection where all stats are 0
            var collection = new StatsCollection(0);
            Assert.AreEqual(0,collection[new Stat("Dangerous")]);
  
            // Invent a new stat and set it to 10
            collection[new Stat("Wild")] = 10;           
            Assert.AreEqual(10,collection[new Stat("Wild")]);



            var world = Setup("stats.yaml",statsYaml,"rooms.yaml",roomYaml);

            // naturally you cannot seduce
            Assert.AreEqual(0,world.Player.BaseStats[world.AllStats.Get("Seduction")]);

            // but in the bar you are more seductive
            Assert.AreEqual(10,world.Player.GetFinalStats()[world.AllStats.Get("Seduction")]);

            //player would be able to equip the helmet of madness if he wanted to
            Assert.IsTrue(world.Player.CurrentLocation.Items[0].RequirementsMet(world.Player));

            // now you are too hot to wear the madness hat
            world.Player.BaseStats[new Stat("Seduction")] = 60;
            Assert.IsFalse(world.Player.CurrentLocation.Items[0].RequirementsMet(world.Player));
        }
    }
}
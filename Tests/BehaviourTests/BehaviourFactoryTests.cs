using NUnit.Framework;
using Tests.Cookbook;
using Wanderer.Items;

namespace Tests.BehaviourTests
{
    class BehaviourFactoryTests : Recipe
    {
        [Test]
        public void TestBehaviour_DoesNothing()
        {
            string behavioursYaml = "- Name: DoNothing";

            var w = Setup("behaviours.yaml",behavioursYaml);

            Assert.AreEqual(1,w.BehaviourFactory.Blueprints.Count);

            var hat = new Item("Hat");

            w.BehaviourFactory.Create(w,hat,w.BehaviourFactory.Blueprints[0]);

            Assert.AreEqual("DoNothing",hat.BaseBehaviours[0].Name);
        }

        [Test]
        public void TestBehaviour_Doomed()
        {
            string behavioursYaml = 
@"
- Name: Doomed
  OnRoundEnding:
     Condition: 
       - return true
     Effect:
       - Recipient:Kill(UserInterface,Round,'doom')
";

            var w = Setup("behaviours.yaml",behavioursYaml);

            Assert.AreEqual(1,w.BehaviourFactory.Blueprints.Count);

            w.BehaviourFactory.Create(w,w.Player,w.BehaviourFactory.Blueprints[0]);

            Assert.IsFalse(w.Player.Dead);
            
            GoWest(w);

            Assert.IsTrue(w.Player.Dead);
            
        }

    }
}

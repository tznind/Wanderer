using System.Text;
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

        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void TestBehaviour_Doomed(bool? condition)
        {
            var sb = new StringBuilder();

            
sb.AppendLine(@"
- Name: Doomed
  OnRoundEnding:");


        if(condition.HasValue)
            if(condition.Value)
                sb.AppendLine(@"
    Condition:
      - Lua: return true");
            else
                sb.AppendLine(@"
    Condition:
      - Lua: return false");

sb.Append(
@"
    Effect:
      - Lua: Recipient:Kill(UserInterface,Round,'doom')
");

            var w = Setup("behaviours.yaml",sb.ToString());

            Assert.AreEqual(1,w.BehaviourFactory.Blueprints.Count);

            w.BehaviourFactory.Create(w,w.Player,w.BehaviourFactory.Blueprints[0]);

            Assert.IsFalse(w.Player.Dead);
            
            GoWest(w);

            Assert.AreEqual(condition ?? true,w.Player.Dead,"Player should be dead if no Conditions or condition is true");
            
        }

    }
}

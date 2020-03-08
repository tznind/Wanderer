using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories;

namespace Tests.BehaviourTests
{
    public class GetsHungryBehaviourTests : UnitTest
    {
        [Test]
        public void TestHunger_Appears()
        {
            var you = YouInARoom(out IWorld world);

            Assert.IsEmpty(you.Adjectives.OfType<IInjured>());

            Assert.AreEqual(1,you.BaseBehaviours.OfType<GetsHungryBehaviour>().Count());

            for(int i=0;i<6;i++)
                world.RunRound(GetUI(),new LoadGunsAction());

            Assert.AreEqual("Peckish",you.Adjectives.OfType<IInjured>().Single().Name);

        }
        
        [Test]
        public void TestHunger_Starvation()
        {
            var you = YouInARoom(out IWorld world);

            Assert.IsEmpty(you.Adjectives.OfType<IInjured>());

            Assert.AreEqual(1,you.BaseBehaviours.OfType<GetsHungryBehaviour>().Count());

            int i;
            for(i=0;i<1000;i++)
                if(you.Dead)
                    break;
                else
                    world.RunRound(GetUI(),new LoadGunsAction());

            Assert.IsTrue(you.Dead);

            //total turns without eating before death
            Assert.AreEqual(61,i);

        }

        
        [Test]
        public void TestHunger_EatingFood()
        {
            var you = YouInARoom(out IWorld world);
            
            for(int i=0;i<6;i++)
                world.RunRound(GetUI(),new LoadGunsAction());

            Assert.AreEqual("Peckish",you.Adjectives.OfType<IInjured>().Single().Name);

            string yaml = @"
- Name: Apple
  MandatoryAdjectives:
    - Type: SingleUse
  Actions:
    - EatAction
";
            
            var itemFactory = new YamlItemFactory(yaml, new AdjectiveFactory());
            you.Items.Add(itemFactory.Create(world, itemFactory.Blueprints[0]));

            Assert.AreEqual(1,you.Items.Count);
            var ui = GetUI();
            world.RunRound(ui,you.Items.Single().BaseActions.Single());

            Assert.Contains("Test Wanderer ate Apple",ui.Log.RoundResults.Select(m=>m.Message).ToArray());

            Assert.IsEmpty(you.Adjectives.OfType<IInjured>().ToArray());
            Assert.AreEqual(0,you.Items.Count);
        }
    }
}

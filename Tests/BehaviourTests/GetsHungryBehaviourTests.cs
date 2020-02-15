using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Adjectives.ActorOnly;
using Wanderer.Behaviours;

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
            Assert.AreEqual(54,i);

        }
    }
}

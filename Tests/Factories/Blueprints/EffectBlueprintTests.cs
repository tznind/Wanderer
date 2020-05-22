using System;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Systems;

namespace Tests.Factories.Blueprints
{
    class EffectBlueprintTests : UnitTest
    {
        private SystemArgs GetSystemArgs(IActor forActor, IHasStats recipient)
        {
            return new SystemArgs(forActor.CurrentLocation.World,GetUI(),0,forActor,recipient,Guid.Empty);
        }

        [Test]
        public void TestEffect_KillPlayer()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var effect = new EffectBlueprint() {Kill = "Nukes"}.Create().Single();
            
            Assert.IsFalse(you.Dead);
            Assert.IsFalse(them.Dead);
            
            effect.Apply(GetSystemArgs(you,them));

            Assert.IsTrue(you.Dead);
            Assert.IsFalse(them.Dead);
        }

        [Test]
        public void TestEffect_KillTarget()
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);

            var effect = new EffectBlueprint() {KillRecipient = "Nukes"}.Create().Single();
            
            Assert.IsFalse(you.Dead);
            Assert.IsFalse(them.Dead);
            
            effect.Apply(GetSystemArgs(you,them));

            Assert.IsFalse(you.Dead);
            Assert.IsTrue(them.Dead);
        }
    }
}

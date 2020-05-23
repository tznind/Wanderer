using System;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Tests.Factories.Blueprints
{
    class EffectBlueprintTests : UnitTest
    {
        private SystemArgs GetSystemArgs(IActor forActor, IHasStats recipient)
        {
            return new SystemArgs(recipient is IRoom r ? r.World : forActor.CurrentLocation.World,GetUI(),0,forActor,recipient,Guid.Empty);
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

            var effect = new EffectBlueprint() 
            {
                Kill = "Nukes",
                Target = SystemArgsTarget.Recipient
            }.Create().Single();
            
            Assert.IsFalse(you.Dead);
            Assert.IsFalse(them.Dead);
            
            effect.Apply(GetSystemArgs(you,them));

            Assert.IsFalse(you.Dead);
            Assert.IsTrue(them.Dead);
        }

        [Test]
        public void TestSet_Variable()
        {
            var room = InARoom(out IWorld world);
            
            var effect = new EffectBlueprint() {Set = "X+5"}.Create().Single();
            effect.Apply(GetSystemArgs(null,room));
            Assert.AreEqual(5,room.V["X"]);
        }
        [Test]
        public void TestSet_Stat()
        {
            var room = InARoom(out IWorld world);
            
            var effect = new EffectBlueprint() {Set = "Fight+5"}.Create().Single();
            effect.Apply(GetSystemArgs(null,room));
            Assert.AreEqual(5,room.BaseStats["Fight"]);
        }
        [Test]
        public void TestSet_Stat_PlusEquals()
        {
            var room = InARoom(out IWorld world);
            
            var effect = new EffectBlueprint() {Set = "Fight+=5"}.Create().Single();
            room.BaseStats["Fight"] = 5;
            
            effect.Apply(GetSystemArgs(null,room));

            Assert.AreEqual(10,room.BaseStats["Fight"]);
        }
        [Test]
        public void TestSet_Stat_PlusPlus()
        {
            var room = InARoom(out IWorld world);
            
            var effect = new EffectBlueprint() {Set = "Fight++"}.Create().Single();
            room.BaseStats["Fight"] = 5;
            
            effect.Apply(GetSystemArgs(null,room));

            Assert.AreEqual(6,room.BaseStats["Fight"]);
        }
    }
}

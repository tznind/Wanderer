using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests.Factories.Blueprints
{
    class ConditionBlueprintTests : UnitTest
    {
        private SystemArgs GetSystemArgs(IActor forActor)
        {
            return new SystemArgs(forActor.CurrentLocation.World,null,0,forActor,forActor,Guid.Empty);
        }

        [Test]
        public void TestCondition_RoomHas_PassBecauseRoomHasName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomHas = "Fish"}.Create().Single();
            
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
            you.CurrentLocation.Name = "Fish";
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
        }
        [Test]
        public void TestCondition_RoomNotHas_PassBecauseRoomHasName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomHasNot = "Fish"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
            you.CurrentLocation.Name = "Fish";
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
        }
        
        [Test]
        public void TestCondition_RoomIs_PassBecauseRoomIsName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomIs = "Fish"}.Create().Single();
            
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));

            you.CurrentLocation.Adjectives.Add(new Adjective(you.CurrentLocation){Name= "Fish"});
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)), "Is should not consider adjectives");

            you.CurrentLocation.Name = "Fish";
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)), "Now Room has the Name to match, it should be considered 'Is'");
        }

        [Test]
        public void TestCondition_RoomIsNot_PassBecauseRoomIsName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomIsNot = "Fish"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));

            you.CurrentLocation.Adjectives.Add(new Adjective(you.CurrentLocation){Name= "Fish"});
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));

            you.CurrentLocation.Name = "Fish";
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)), "Now Room has the Name to match, it should be considered 'Is' therefore Not should make this codition true");
        }



        #region Actor Tests
        [Test]
        public void TestCondition_Has_PassBecauseHasAdjective()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {Has = "Fish"}.Create().Single();
            
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
            you.Adjectives.Add(new Adjective(you){Name = "Fish"});

            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
        }
        [Test]
        public void TestCondition_HasNot_PassBecauseHasAdjective()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {HasNot = "Fish"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
            you.Adjectives.Add(new Adjective(you){Name = "Fish"});

            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
        }

        [Test]
        public void TestCondition_Is_PassBecauseIsType()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {Is = "Fish"}.Create().Single();
            
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
            you.Adjectives.Add(new Adjective(you){Name = "Fish"});

            condition = new ConditionBlueprint() {Is = "You"}.Create().Single();
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)),"Should be true because Is matches on Type name 'You'");
        }


        [Test]
        public void TestCondition_IsNot_PassBecauseIsType()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {IsNot = "Fish"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
            you.Adjectives.Add(new Adjective(you){Name = "Fish"});

            condition = new ConditionBlueprint() {IsNot = "You"}.Create().Single();
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)),"Should be false because Is matches on Type name 'You' and condition is 'Not'");
        }
        #endregion

        [Test]
        public void TestCondition_Variable_NotEqual()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {Variable = "Counter != 5"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));

            you.V["Counter"] = 3;

            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));

            you.V["Counter"] = 5;

            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));

            you.V["Counter"] = 7;

            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));

        }
    }
}

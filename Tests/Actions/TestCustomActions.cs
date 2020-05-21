using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;

namespace Tests.Actions
{
    class TestCustomActions : UnitTest
    {
        [Test]
        public void TestActionEffect_Dead()
        {
            var blue = new ActionBlueprint()
            {
                Name = "Drink",
                Effect = new List<EffectBlueprint>(new[] {new EffectBlueprint{Lua = "Recipient:Kill(UserInterface,Round,'Poison')"}})
            };

            var you = YouInARoom(out IWorld world);

            Assert.IsFalse(you.Dead);
            world.ActionFactory.Create(world,you,blue);

            var ui = GetUI();
            world.RunRound(ui,you.GetFinalActions().Single(a=>a.Name.Equals("Drink")));

            Assert.IsTrue(you.Dead);
            Assert.Contains("You died of Poison",ui.MessagesShown);
        }
    }
}

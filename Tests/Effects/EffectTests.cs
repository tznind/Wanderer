using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Tests.Effects
{
    class EffectTests : UnitTest
    {
        [Test]
        public void TestEffect_StatBoost()
        {
            var you = YouInARoom(out IWorld world);

            string yaml =
                @"
Body: 
    - Text: Would you like a stat bonus?
Options:   
    - Text: Yes Please!
      Effect: 
        - Lua: AggressorIfAny.BaseStats:Increase(Fight , 20)
";
            var blue = Compiler.Instance.Deserializer.Deserialize<DialogueNodeBlueprint>(yaml);
            var factory = new DialogueNodeFactory();
            var n = factory.Create(blue);

            var d = new DialogueSystem();
            d.AllDialogues.Add(n);

            //pick the first option
            var ui = GetUI(n.Options.First());

            double before = you.BaseStats[Stat.Fight];
            d.Run(new SystemArgs(world,ui,0,you,Mock.Of<IHasStats>(),Guid.NewGuid()),n);
            Assert.AreEqual(before + 20,you.BaseStats[Stat.Fight]);


        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void TestEffect_SetNextDialogue(bool setNull)
        {
            TwoInARoom(out You you, out IActor them,out IWorld world);
            
            var before = them.Dialogue.Next = Guid.NewGuid();
            
            string yaml =
                @$"
Body: 
    - Text: Hey there
Options:   
    - Text: Hey yourself
      Effect: 
        - Lua: Recipient.Dialogue.Next = {(setNull ? "null": $"Guid('{Guid.NewGuid()}')")}
";
            var blue = Compiler.Instance.Deserializer.Deserialize<DialogueNodeBlueprint>(yaml);
            var factory = new DialogueNodeFactory();
            var n = factory.Create(blue);

            var d = new DialogueSystem();
            d.AllDialogues.Add(n);

            //pick the first option
            var ui = GetUI(n.Options.First());
            d.Run(new SystemArgs(world,ui,0,you,them,Guid.NewGuid()),n);

            if(setNull)
                Assert.IsNull(them.Dialogue.Next);
            else
            {
                Assert.IsNotNull(them.Dialogue.Next);
                Assert.AreNotEqual(before,them.Dialogue.Next);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Compilation;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace Tests.Effects
{
    class EffectTests : UnitTest
    {
        [Test]
        public void TestStatBoost()
        {
            var you = YouInARoom(out IWorld _);

            string yaml =
                @"
Body: 
    - Text: Would you like a stat bonus?
Options:   
    - Text: Yes Please!
      Effect: 
        - AggressorIfAny.StatEffect<IActor>(Fight,20)
";
            var n = Compiler.Instance.Deserializer.Deserialize<DialogueNode>(yaml);

            var d = new DialogueSystem();
            d.AllDialogues.Add(n);

            //pick the first option
            var ui = GetUI(n.Options.First());

            double before = you.BaseStats[Stat.Fight];
            d.Run(new SystemArgs(ui,0,you,Mock.Of<IHasStats>(),Guid.NewGuid()),n);
            Assert.AreEqual(before + 20,you.BaseStats[Stat.Fight]);


        }
    }
}

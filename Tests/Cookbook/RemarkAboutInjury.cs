using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Adjectives;
using Wanderer.Systems;

namespace Tests.Cookbook
{
    class RemarkAboutInjury : Recipe
    {
        private string dialogueYaml = 
@"
- Body: 
   - Text: Greetings berk
   - Text: that's a nasty looking cut you got there
     Condition: 
       - return AggressorIfAny:Has('Injured')
";

        [Test]
        public void ConfirmRecipe()
        {
            var world = SetupDialogue(dialogueYaml);
            
            Assert.AreEqual("Greetings berk",RunDialogue(world));

            //give them an injury from the injury system mentioned
            world.Player.Adjectives.Add(new Injured("Cut face",world.Player,10,InjuryRegion.Eye,new InjurySystem()));

            Assert.AreEqual("Greetings berk that's a nasty looking cut you got there",RunDialogue(world));

        }
    }
}

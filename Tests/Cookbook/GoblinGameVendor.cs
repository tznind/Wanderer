using System.Linq;
using NUnit.Framework;
using Wanderer;

namespace Tests.Cookbook
{
    class GoblinGameVendor : Recipe
    {

    string injurySystemYaml = 
            @"
Name: Interest
FatalThreshold: 40
FatalVerb: excitement
Injuries:
   - Name: Interested
     Severity: 10
   - Name: Excited
     Severity: 20
   - Name: Mind Blown
     Region: Head
     Severity: 30
";

string room = @"
- Name: Decrepit shop
  MandatoryActors:
    - Name: Goblin
      Dialogue:
        Next: 9d07cdba-f7bf-47c3-9039-04df8102bd63
  MandatoryItems:
    - Name: Tattered Pamphlet
      Dialogue:
        Verb: Examine
        Next: 58baccad-f794-497b-9674-ae7d4d1005cb
";

string dialogue = @"

- Identifier: 9d07cdba-f7bf-47c3-9039-04df8102bd63
  Body:
  - Text: Have you heard the good news?
  Options:
  - Text: No, and I don't want to either
    Destination: 260d85d8-5924-44fc-9b33-c9114fca29a5
  - Text: '*sigh* go on then'
    Destination: b16df095-43b5-4664-9ba5-f11fd4f9b677
- Identifier: 260d85d8-5924-44fc-9b33-c9114fca29a5
  Body:
  - Text: But... but... this game engine is to die for!
  Options:
  - Text: No I'm not interested at all
  - Text: '... fine, but this had better be good'
    Destination:  b16df095-43b5-4664-9ba5-f11fd4f9b677
- Identifier: b16df095-43b5-4664-9ba5-f11fd4f9b677
  Body:
  - Text: Yes! well... its a game engine you can program entirely in YAML! In fact the code your reading makes a valid world, good eh?... with autocomplete schemas and systems for everything and and and!!! its just so exciting!!
  Options:
  - Text: Yes!!!! I've alway wanted to program in the most whitespace aware and annoying markup language ever!
    Effect:
    - Apply:
         Name: Interest
         Intensity: 9999
  - Text: No.... just No!
    Attitude: -20
";

string dialogue2 = @"
- Identifier: 58baccad-f794-497b-9674-ae7d4d1005cb
  Body:
  - Text: ""The torn and faded lettering seems to witter on at great length about a game engine called 'Wanderer'.  It has a link towards the bottom: https://github.com/tznind/Wanderer""
";

        [TestCase(true)]
        [TestCase(false)]
        public void ConfirmRecipe(bool goViaUnsure)
        {
            var world = Setup("interest.injury.yaml",injurySystemYaml,"goblin.rooms.yaml",room, "dialogue.yaml",dialogue, "pamphlet.dialogue.yaml",dialogue2);
            Assert.AreEqual(1,world.InjurySystems.Count);

            RunRound(world,"PickUp","Tattered Pamphlet");

            Assert.IsFalse(world.Player.Dead);

            RunRound(world,"Examine:Tattered Pamphlet");

            FixedChoiceUI ui;

            if(goViaUnsure)
                RunRound(world,out ui,"talk:Goblin","No, and I don't want to either","... fine, but this had better be good","Yes!!!! I've alway wanted to program in the most whitespace aware and annoying markup language ever!");
            else
                RunRound(world,out ui,"talk:Goblin","*sigh* go on then","Yes!!!! I've alway wanted to program in the most whitespace aware and annoying markup language ever!");

            Assert.IsTrue(world.Player.Dead);
            Assert.Contains("You died of excitement",ui.Log.RoundResults.Select(r=>r.Message).ToArray());
        }
    }
}

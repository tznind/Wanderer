using System;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Places;
using Wanderer.Systems;
using Wanderer.Validation;

namespace Tests.Validation
{
    class WorldValidatorTests
    {
        [Test]
        public void TestWorldValidator_Success()
        {
            var v = new WorldValidator();

            v.Validate(new WorldFactory());

            Assert.IsEmpty(v.Errors.ToString());
            Assert.IsEmpty(v.Warnings.ToString());
        }

        [Test]
        public void TestWorldValidator_MissingDialogue()
        {
            var v = new WorldValidator();
            var f = new WorldFactory();

            var w = f.Create();

            w.Dialogue.AllDialogues.Clear();
            v.Validate(w);
            
            StringAssert.Contains("Could not find Dialogue",v.Errors.ToString());
        }

        [Test]
        public void TestWorldValidator_BadEffectCode()
        {
            var w = new WorldFactory().Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Require = {new ConditionCode<SystemArgs>("Troll = 1")}
            };
            w.Dialogue.AllDialogues.Add(d);

            v.Validate(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("Error testing dialogue condition on '1cf15faf-837b-4629-84c5-bdfa7631a905'",v.Warnings.ToString());
            StringAssert.Contains("The name 'Troll' does not exist in the current context",v.Warnings.ToString());
        }
    }
}
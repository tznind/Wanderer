using System;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Plans;
using Wanderer.Systems;
using Wanderer.Systems.Validation;

namespace Tests
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
        public void TestWorldValidator_BadConditionCode()
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

        [Test]
        public void TestWorldValidator_DialogueWithNoText()
        {
            var w = new WorldFactory().Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")
            };

            w.Dialogue.AllDialogues.Add(d);

            v.Validate(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905' has no Body Text",v.Errors.ToString());
        }



        [Test]
        public void TestWorldValidator_DialogueOptionWithNoText()
        {
            var w = new WorldFactory().Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new TextBlock[]
                {
                    new TextBlock("I dare say")
                },
                Options = {new DialogueOption()}
            };
            w.Dialogue.AllDialogues.Add(d);

            v.Validate(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("A Dialogue Option of Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905' has no Text",v.Errors.ToString());
        }



        [TestCase("Trollolol=1","The name 'Trollolol' does not exist in the current context")] // bad runtime value
        [TestCase("sdf sdf sdf","; expected")] // bad compile time value
        public void TestWorldValidator_DialogueOptionWithBadEffectCode(string badCode, string expectedError)
        {
            var w = new WorldFactory().Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new TextBlock[]
                {
                    new TextBlock("I dare say")
                },
                Options = {
                    new DialogueOption()
                    {
                        Text = "Do stuff",
                        Effect = 
                        {
                            new EffectCode(badCode)
                        }
                    }
                }
            };
            w.Dialogue.AllDialogues.Add(d);

            v.Validate(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
           
          
            StringAssert.Contains("Error testing EffectCode of Option 'Do stuff' of Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905'",v.Warnings.ToString());
            StringAssert.Contains(expectedError,v.Warnings.ToString());
        }

                [Test]
        public void TestWorldValidator_DialogueOptionWithMissingDestination()
        {
            var w = new WorldFactory().Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new TextBlock[]
                {
                    new TextBlock("I dare say")


                },
                Options = {
                    new DialogueOption()
                    {
                        Text = "Rather!",

                        //This is missing
                        Destination = new Guid("d7bcff5f-31a4-41ad-a71e-9b51a6565fc3")
                    }
                }
            };
            w.Dialogue.AllDialogues.Add(d);

            v.Validate(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("Could not find Dialogue 'd7bcff5f-31a4-41ad-a71e-9b51a6565fc3",v.Errors.ToString());
        }

        [Test]
        public void TestValidate_BadPlanCondition()
        {
            WorldValidator v = new WorldValidator();
            
            var plan = new Plan()
            {
                Condition = 
                {
                    new ConditionCode<SystemArgs>("throw new Exception(\"this is bat country!\");")
                }
            };

            v.Validate(Mock.Of<IWorld>(),plan, Mock.Of<IActor>());

            StringAssert.Contains("One or more errors occurred. (this is bat country!)",v.Warnings.ToString());
        }


        [Test]
        public void TestValidate_BadPlanMissingDoFrame()
        {
            WorldValidator v = new WorldValidator();
            
            var plan = new Plan()
            {
                Name = "Do something nefarious",
                Condition = 
                {
                    new ConditionCode<SystemArgs>("true")
                }
            };

            v.Validate(Mock.Of<IWorld>(),plan, Mock.Of<IActor>());

            StringAssert.Contains("Plan 'Do something nefarious' has no DoFrame",v.Errors.ToString());
        }



        [Test]
        public void TestValidate_BadPlanBadDoFrame()
        {
            WorldValidator v = new WorldValidator();
            
            var plan = new Plan()
            {
                Name = "Do something nefarious",
                Condition = 
                {
                    new ConditionCode<SystemArgs>("true")
                },
                Do = new FrameSourceCode("fffff")
            };

            v.Validate(Mock.Of<IWorld>(),plan, Mock.Of<IActor>());

            StringAssert.Contains(@"Failed to validate DoFrame of Plan 'Do something nefarious'",v.Warnings.ToString());
            StringAssert.Contains(@"(1,1): error CS0103: The name 'fffff' does not exist",v.Warnings.ToString());
        }
    }
}
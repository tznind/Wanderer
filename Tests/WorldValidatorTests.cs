﻿using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Plans;
using Wanderer.Rooms;
using Wanderer.Systems;
using Wanderer.Systems.Validation;

namespace Tests
{
    class WorldValidatorTests
    {
        public string EmptyDir => Path.Combine(TestContext.CurrentContext.WorkDirectory, "EmptyDir");
        public string NormalResourcesDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources");

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.CreateDirectory(EmptyDir);
        }

        [Test]
        public void TestWorldValidator_Success()
        {
            var v = new WorldValidator();
            v.IncludeStackTraces = true;
            v.Validate(new WorldFactory()
            {
                ResourcesDirectory = NormalResourcesDirectory
            });

            Assert.IsEmpty(v.Errors.ToString());
            Assert.IsEmpty(v.Warnings.ToString());

            Assert.AreEqual(0,v.WarningCount);
            Assert.AreEqual(0,v.ErrorCount);
        }

        

        [Test]
        public void TestWorldValidator_MissingDialogue()
        {
            var v = new WorldValidator();
            var f = new WorldFactory
            {
                ResourcesDirectory = NormalResourcesDirectory
            };
            
            var w = f.Create();

            w.Dialogue.AllDialogues.Clear();
            w.RoomFactory.Blueprints.Add(new RoomBlueprint()
            {
                Dialogue = new DialogueInitiation()
                {
                    Next = Guid.NewGuid()
                }
            });
            v.Validate(w);

            Assert.AreEqual(v.ErrorCount,1);
            
            StringAssert.Contains("Could not find Dialogue",v.Errors.ToString());
        }

        [Test]
        public void TestWorldValidator_BadConditionCode()
        {
            var w = new WorldFactory {ResourcesDirectory = EmptyDir}.Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Condition = {new ConditionCode("Tr oll oll = 1")}
            };
            w.Dialogue.AllDialogues.Add(d);

            v.ValidateDialogue(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("Error testing dialogue condition on '1cf15faf-837b-4629-84c5-bdfa7631a905'",v.Warnings.ToString());
            StringAssert.Contains("<eof> expected near 'oll'",v.Warnings.ToString());
        }

        [Test]
        public void TestWorldValidator_DialogueWithNoText()
        {
            var w = new WorldFactory{ResourcesDirectory = EmptyDir}.Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")
            };

            w.Dialogue.AllDialogues.Add(d);

            v.ValidateDialogue(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
            
            StringAssert.Contains("Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905' has no Body Text",v.Errors.ToString());
        }



        [Test]
        public void TestWorldValidator_DialogueOptionWithNoText()
        {
            var world = 
                new WorldFactory {ResourcesDirectory = EmptyDir}
                    .Create();

            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new List<TextBlock>
                {
                    new TextBlock("I dare say")
                },
                Options = {new DialogueOption()}
            };
            world.Dialogue.AllDialogues.Add(d);

            v.ValidateDialogue(world,world.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },world.Player.CurrentLocation );
            
            StringAssert.Contains("A Dialogue Option of Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905' has no Text",v.Errors.ToString());
        }



        [TestCase("sdf sdf sdf","syntax error near 'sdf'")] // bad compile time value
        public void TestWorldValidator_DialogueOptionWithBadEffectCode(string badCode, string expectedError)
        {
            var w = new WorldFactory{ResourcesDirectory = EmptyDir}.Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new List<TextBlock>
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

            v.ValidateDialogue(w,w.Player,new DialogueInitiation()
            {
                Next = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905")

            },w.Player.CurrentLocation );
           
          
            StringAssert.Contains("Error testing EffectCode of Option 'Do stuff' of Dialogue '1cf15faf-837b-4629-84c5-bdfa7631a905'",v.Warnings.ToString());
            StringAssert.Contains(expectedError,v.Warnings.ToString());
        }

                [Test]
        public void TestWorldValidator_DialogueOptionWithMissingDestination()
        {
            var w = new WorldFactory{ResourcesDirectory = EmptyDir}.Create();
            var v = new WorldValidator();

            var d = new DialogueNode()
            {
                Identifier = new Guid("1cf15faf-837b-4629-84c5-bdfa7631a905"),
                Body = new List<TextBlock>
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

            v.ValidateDialogue(w,w.Player,new DialogueInitiation()
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
                    new ConditionCode("this is bat country")
                }
            };

            v.Validate(Mock.Of<IWorld>(),plan, Mock.Of<IActor>());

            StringAssert.Contains("Error executing 'ConditionCode' script code 'return this is bat country'.",v.Warnings.ToString());
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
                    new ConditionCode("true")
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
                    new ConditionCode("true")
                },
                Do = new FrameSourceCode("fffff")
            };

            v.Validate(Mock.Of<IWorld>(),plan, Mock.Of<IActor>());

            StringAssert.Contains(@"Failed to validate DoFrame of Plan 'Do something nefarious'",v.Warnings.ToString());
        }

        [Test]
        public void TestValidate_SlotNotFound()
        {
            WorldValidator v = new WorldValidator();

            var w = new World();
            var blue = new ItemBlueprint
            {
                Name = "Hammer",
                Slot = new ItemSlot("Handers",5)
            };

            v.ValidateItem(w,w.ItemFactory.Create(w,blue), Mock.Of<IRoom>());

            StringAssert.Contains(@"Item Hammer lists Slot named Handers but no Actors or Default slots are listed with that name (Slots seen were '')",v.Warnings.ToString());
        }
        
        [Test]
        public void TestValidate_BadBehaviour()
        {
            WorldValidator v = new WorldValidator();

            var w = new World();
            var blue = new ItemBlueprint
            {
                Name = "Hammer",
                Behaviours = 
                    new List<BehaviourBlueprint>()
                    {
                        new BehaviourBlueprint()
                        {
                            OnPush = new BehaviourEventHandlerBlueprint()
                            {
                                Effect = new List<EffectBlueprint>{new EffectBlueprint{Lua = "aa + bb + cc"}}
                            }
                        }
                    }
            };

            v.ValidateItem(w,w.ItemFactory.Create(w,blue), new Room("Test Room",w,'t'));

            StringAssert.Contains(@"Error testing OnPush of Behaviour Unnamed Object of on 'Hammer' in room 'Test Room' with test actor 'test actor'",v.Warnings.ToString());
        }
        
        [Test]
        public void TestWorldValidator_BadYaml()
        {
            var dir = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory, "BadYamlDir"));

            if(dir.Exists)
                dir.Delete(true);

            dir.Create();

            var f = new WorldFactory()
            {
                ResourcesDirectory = dir.FullName
            };

            File.WriteAllText(Path.Combine(dir.FullName,"rooms.yaml"),"ffffff");
            
            var v = new WorldValidator();
            v.Validate(f);
            StringAssert.Contains("Error Creating World",v.Errors.ToString());
            StringAssert.Contains("Error loading RoomBlueprint in file",v.Errors.ToString());
            StringAssert.Contains("rooms.yaml",v.Errors.ToString());
            

            Assert.IsEmpty(v.Warnings.ToString());
            Assert.AreEqual(1,v.ErrorCount);
            Assert.AreEqual(0,v.WarningCount);
        
        }
    }
}
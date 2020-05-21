using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;

namespace Tests.Actions
{
    public class ActionTests
    {
        [Test]
        public void AllActionsHaveDefaultConstructor()
        {
            foreach (var a in Compiler.Instance.TypeFactory.Create<IAction>())
            {
                //TODO weird corner case not sure why this is even an action
                if(a == typeof(ForbidAction))
                    continue;

                Assert.IsNotNull(Activator.CreateInstance(a,Mock.Of<IActor>()),$"Could not find default constructor on {a}");
            }
        }
        [Test]
        public void AllActionsHaveUniqueHotkeys()
        {
            Dictionary<char,IAction> actionKeys = new Dictionary<char, IAction>();

            foreach (var a in Compiler.Instance.TypeFactory.Create<IAction>())
            {
                //TODO weird corner case not sure why this is even an action
                if(a == typeof(ForbidAction))
                    continue;

                var inst = ((IAction)Activator.CreateInstance(a,Mock.Of<IActor>()));
                var key = inst.HotKey;
                
                if(actionKeys.ContainsKey(key))
                    Assert.Fail($"Actions {actionKeys[key]} and {inst} share the same HotKey {key}");
                else
                    actionKeys.Add(key,inst);
            }
        }
    }

    public class TestAction : Wanderer.Actions.Action
    {
        public TestAction(IHasStats owner):base(owner)
        {

        }
        
        public override void Push(IWorld world, IUserinterface ui, ActionStack stack, Wanderer.Actors.IActor actor)
        {
            if(ui.GetChoice("Destroy the world",null,out bool chosen,new []{true,false}))
                if(chosen)
                {
                    stack.Push(new Frame(actor,this,0));
                }
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            world.Player.Kill(ui,stack.Round,"fate");
        }
    }
    
    public class TestCustomAction
    {

        string yaml = @"
Name: Unleash
Type: TestAction";

        [Test]
        public void TestCustomCSharpAction_InSeperateAssembly()
        {
            var old = Compiler.Instance.TypeFactory;

            try
            {
                // Setup the TypeFactory to use both the main Wanderer assembly and our assembly
                Compiler.Instance.TypeFactory = new TypeCollectionFactory(typeof(Compiler).Assembly,typeof(TestAction).Assembly);

                var blue = Compiler.Instance.Deserializer.Deserialize<ActionBlueprint>(yaml);

                var dir = Path.Combine( TestContext.CurrentContext.TestDirectory,"EmptyFolder");
                Directory.CreateDirectory(dir);

                var wf = new WorldFactory(){ResourcesDirectory = dir};
                var world = wf.Create();

                var f = new ActionFactory();
                var action = f.Create(world,world.Player,blue);
                
                Assert.IsInstanceOf(typeof(TestAction),action);
            }
            finally
            {
                Compiler.Instance.TypeFactory = old;
            }


        }
    }    



}
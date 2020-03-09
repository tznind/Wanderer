using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer.Actions;
using Wanderer.Behaviours;
using Wanderer.Compilation;

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

                Assert.IsNotNull(Activator.CreateInstance(a),$"Could not find default constructor on {a}");
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

                var inst = ((IAction)Activator.CreateInstance(a));
                var key = inst.HotKey;
                
                if(actionKeys.ContainsKey(key))
                    Assert.Fail($"Actions {actionKeys[key]} and {inst} share the same HotKey {key}");
                else
                    actionKeys.Add(key,inst);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using NLua;
using NUnit.Framework;
using Wanderer;
using Wanderer.Compilation;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Tests.CompilerTests
{
    class TestLua : UnitTest
    {
        [Test]
        public void TestLua_BasicScript()
        {
            using (Lua lua = new Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoString("res = 'Файл'");
                string res = (string)lua["res"];

                Assert.AreEqual("Файл", res);
            }
        }
        [Test]
        public void TestLua_HasStat()
        {
            var you = YouInARoom(out IWorld world);

            using (Lua lua = new Lua())
            {
                lua["you"] = you;

                lua.LoadCLRPackage ();
                lua.DoString (@"
import ('Wanderer', 'Wanderer')
import ('Wanderer','Wanderer.Stats')
"
                    );

                lua.DoString("result = you:GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsFalse((bool)lua["result"]);

                you.BaseStats[Stat.Corruption] = 6000;
                
                lua.DoString("result = you:GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsTrue((bool)lua["result"]);
            }
        }


        public class A
        {
            public string SayHi()
            {
                return "hi";
            }
            public string SayHiToStr(string tome)
            {
                return "hi " + tome;
            }
            public string SayHiTo(B b)
            {
                return "hi B";
            }
        }

        public class B
        {

        }
        
        [Test]
        public void TestLua_MethodCall()
        {
            using (Lua lua = new Lua())
            {
                var a = new A();
                var b = new B();
                lua["a"] = a;
                lua["b"] = b;
                
                lua.LoadCLRPackage();
                lua.DoString(
                    @$"
import ('System', 'System.String')
import ('{typeof(A).Assembly.GetName().Name}', '{typeof(A).Namespace}')
");
                lua.DoString("result = a:SayHi()");
                Assert.AreEqual("hi", lua["result"]);

                lua.DoString("result = a:SayHiToStr('Goblin')");
                Assert.AreEqual("hi Goblin", lua["result"]);

                lua.DoString("result = a:SayHiTo(b)");
                Assert.AreEqual("hi B", lua["result"]);
            }
        }

        [Test]
        public void TestLua_SubProperties()
        {
            
            var you = YouInARoom(out IWorld world);

            using (Lua lua = new Lua())
            {
                lua["you"] = you;
                lua["location"] = you.CurrentLocation;

                lua.LoadCLRPackage();
                lua.DoString(@"
import ('Wanderer', 'Wanderer')
import ('Wanderer','Wanderer.Stats')
import ('Wanderer','Wanderer.Places')
"
                );

                lua.DoString("result = location:GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsFalse((bool) lua["result"]);

                you.CurrentLocation.BaseStats[Stat.Corruption] = 6000;

                lua.DoString("result = you.CurrentLocation:GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsTrue((bool) lua["result"]);
            }
        }

        [Test]
        public void TestLua_SystemArgsCondition()
        {
            var you = YouInARoom(out IWorld world);

            var code = new ConditionCode<SystemArgs>("condition = place:GetFinalStats(you)[Stat.Corruption] > 50");

            var args = new SystemArgs(world,GetUI(),0,null,you,Guid.Empty);

            var lua = code.GetLua(args);

            lua.DoString(code.CsharpCode);
            Assert.AreEqual(false,lua["condition"]);
        }
    }
}

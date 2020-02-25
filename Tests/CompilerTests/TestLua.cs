using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLua;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Tests.CompilerTests
{

    public class C
    {
        public string Val { get; }

        public C()
        {
            Val = "trollolol";
        }
        public C(string val)
        {
            Val = val;
        }
        public C(int val)
        {
            Val = val.ToString() + "(int overload used)";
        }
    }

    public class D
    {
        public Guid G {get;set;}
        public D(string g)
        {
            G = new Guid(g);
        }
    }
    
    public class SomeClass
    {
        public string SayHi()
        {
            return "Hi";
        }
    }

    
    public class SomeListClass :List<string>
    {
        public string SayHi()
        {
            return "Hi";
        }
    }
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

            var code = new ConditionCode<SystemArgs>("return Place:GetFinalStats(AggressorIfAny)[Corruption] > 50");

            var args = new SystemArgs(world,GetUI(),0,null,you,Guid.Empty);

            Assert.AreEqual(false,code.IsMet(world,args));
        }

        [Test]
        public void TestLua_Assignment()
        {
            var you = YouInARoom(out IWorld world);

            Assert.AreEqual("TestRoom",you.CurrentLocation.Name);
            var code = new ConditionCode<SystemArgs>("Place.Name = 'fish'");

            var args = new SystemArgs(world,GetUI(),0,null,you,Guid.Empty);

            using(var lua = code.Factory.Create(world,args))
            {
               lua.DoString(code.Script);
               Assert.AreEqual("fish",you.CurrentLocation.Name);
            }
        }

        [Test]
        public void TestLua_ConstructGuid()
        {
            using(var lua =  new Lua())
            {
                lua.LoadCLRPackage();
                lua.DoString(@"import ('System','System')");
                lua.DoString($"import ('{typeof(C).Assembly.GetName().Name}','{typeof(C).Namespace}')");
                lua.DoString($"import ('{typeof(Guid).Assembly.GetName().Name}','{typeof(Guid).Namespace}')");

                Assert.AreEqual("fff",lua.DoString("return 'fff'")[0]);

                Assert.AreEqual("trollolol",((C)lua.DoString("return C()")[0]).Val);
                Assert.AreEqual("fff",((C)lua.DoString("return C('fff')")[0]).Val);
                Assert.AreEqual("5(int overload used)",((C)lua.DoString("return C(5)")[0]).Val);

                lua.DoString("GuidClass=luanet.import_type('System.Guid')");
                lua.DoString("guid_cons=luanet.get_constructor_bysig(GuidClass,'System.String')");
                
                Assert.AreEqual(
                    new Guid("adc70ae1-769e-4ace-aa83-928a604c5739"),
                lua.DoString("return guid_cons('adc70ae1-769e-4ace-aa83-928a604c5739')")[0]);

                Assert.AreEqual(
                    new Guid("adc70ae1-769e-4ace-aa83-928a604c5739"),
                    ((D)lua.DoString("return D('adc70ae1-769e-4ace-aa83-928a604c5739')")[0]).G);

                Assert.IsNotNull(lua.DoString("return Guid.NewGuid()")[0]);
                Assert.AreEqual(Guid.Empty,lua.DoString("return Guid.Empty")[0]);
                Assert.AreEqual(
                    new Guid("adc70ae1-769e-4ace-aa83-928a604c5739"),
                lua.DoString("return Guid.Parse('adc70ae1-769e-4ace-aa83-928a604c5739')")[0]);


                
                LuaFactory.ApplyGuidConstructorFix(lua);

                Assert.AreEqual(
                    new Guid("adc70ae1-769e-4ace-aa83-928a604c5739"),
                lua.DoString("return Guid('adc70ae1-769e-4ace-aa83-928a604c5739')")[0]);
            }
        }



        [Test]
        public void TestLua_Methods()
        {
            using (var lua = new Lua())
            {
                lua.LoadCLRPackage();
                lua.DoString(@"import ('System','System')");
                lua.DoString($"import ('{typeof(C).Assembly.GetName().Name}','{typeof(C).Namespace}')");

                lua["SomeClass1"] = new SomeClass();
                Assert.AreEqual("Hi",lua.DoString("return SomeClass1:SayHi()")[0]);
                
                lua["SomeListClass1"] = new SomeListClass();
                Assert.AreEqual("Hi",lua.DoString("return SomeListClass1:SayHi()")[0]);

            }
        }
        
        [Test]
        public void TestLua_CallMainLuaMethod()
        {
            var you = YouInARoom(out IWorld world);

            var f = new LuaFactory();
            
            using (var lua = f.Create(world, null))
            {
                lua["you"] = you;
                
                Assert.IsNull(lua.DoString("return GetFirstEquippableItem(you)")[0]);

                you.AvailableSlots = new SlotCollection{{"Head",1}};
                you.Items.Add(new Item("Hat") {Slot = new ItemSlot("Head", 1)});

                Assert.IsTrue(you.CanEquip(you.Items.Single(),out _));
                
                Assert.AreEqual(you.Items.Single(),lua.DoString("return GetFirstEquippableItem(you)")[0]);

                you.AvailableSlots.Clear();

                Assert.IsNull(lua.DoString("return GetFirstEquippableItem(you)")[0]);
            }
        }
    }
}

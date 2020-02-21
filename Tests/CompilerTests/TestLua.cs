using System;
using System.Collections.Generic;
using System.Text;
using NLua;
using NUnit.Framework;
using Wanderer;
using Wanderer.Stats;

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

                lua.DoString("result = you.GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsFalse((bool)lua["result"]);

                you.BaseStats[Stat.Corruption] = 6000;
                
                lua.DoString("result = you.GetFinalStats(you)[Stat.Corruption] > 50");
                Assert.IsTrue((bool)lua["result"]);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog;
using NLua;
using Wanderer.Stats;

namespace Wanderer.Factories
{
    public class LuaFactory
    {
        private static readonly Stopwatch sw = new Stopwatch();
        public Lua Create(IWorld world,object o)
        {
            sw.Start();
            var lua = Create();

            lua["this"] = o;

            if (o != null)
            {
                foreach(var prop in o.GetType().GetProperties())
                {
                    var val = prop.GetValue(o);

                    if(val != null)
                        lua[prop.Name] = val;
                }
            }
                


            var main = Path.Combine(world.ResourcesDirectory, "Main.lua");
            if (File.Exists(main)) 
                lua.DoFile(main);

            ApplyGuidConstructorFix(lua);
            
            sw.Stop();

            LogManager.GetCurrentClassLogger().Log(LogLevel.Trace, "Cumulative lua create time:" + sw.ElapsedMilliseconds.ToString("N0"));

            return lua;
        }

        protected Lua Create()
        {
            var lua =  new Lua();

                lua.LoadCLRPackage();
                lua.DoString(@"
import ('Wanderer', 'Wanderer')
import ('Wanderer','Wanderer.Stats')
import ('Wanderer','Wanderer.Rooms')
import ('Wanderer','Wanderer.Actions')
import ('System','System')
import ('Wanderer','Wanderer.Systems')
import ('Wanderer','Wanderer.Adjectives')
import ('Wanderer','Wanderer.Items')

");

            //setup Fight as alias for Stat.Fight etc
            foreach(Stat val in Enum.GetValues(typeof(Stat)))
                if(val != Stat.None)
                    lua[val.ToString()] = val;

            return lua;
        }

        public static void ApplyGuidConstructorFix(Lua lua)
        {
            //TODO: this is a hacky workaround for guid constructor seemingly returning null
            lua.DoString("GuidClass=luanet.import_type('System.Guid')");
            lua.DoString("Guid=luanet.get_constructor_bysig(GuidClass,'System.String')");
        }
    }
}

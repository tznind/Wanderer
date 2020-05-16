using System;
using System.IO;
using NLua;
using Wanderer.Stats;

namespace Wanderer.Factories
{
    public class LuaFactory
    {
        public Lua Create(IWorld world,object o)
        {
            var lua = Create();
            
            //setup Fight as alias for Stat.Fight etc
            foreach(Stat val in world.AllStats.All)
                lua[val.ToString()] = val;

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
                
            if (world.MainLua != null) 
                lua.DoString(world.MainLua);

            ApplyGuidConstructorFix(lua);

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

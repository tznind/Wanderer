using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using NLua;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Code 
    {
        public string CsharpCode { get; set; }

        
        [JsonConstructor]
        public Code()
        {
        }

        public Code(string csharpCode)
        {
            CsharpCode = csharpCode;
        }


        protected ScriptOptions GetScriptOptions()
        {
            return ScriptOptions.Default
                .WithReferences(typeof(ICondition<>).Assembly)
                .WithImports(
                    "Wanderer.Stats", 
                    "System",
                    "System.Linq",
                    "Wanderer",
                    "Wanderer.Actors",
                    "Wanderer.Actions",
                    "Wanderer.Systems",
                    "Wanderer.Adjectives");
        }

        public Lua GetLua(SystemArgs args)
        {
            var lua = GetLua();
            

            lua["place"] = args.Place;

            return lua;
        }

        protected Lua GetLua()
        {
            var lua =  new Lua();

                lua.LoadCLRPackage();
                lua.DoString(@"
import ('Wanderer', 'Wanderer')
import ('Wanderer','Wanderer.Stats')
import ('Wanderer','Wanderer.Places')
import ('Wanderer','Wanderer.Actions')
import ('System','System')
import ('Wanderer','Wanderer.Systems')
import ('Wanderer','Wanderer.Adjectives')

");
            return lua;
        }

        
        public override string ToString()
        {
            return CsharpCode ;
        }
    }
}
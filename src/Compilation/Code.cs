using System.Diagnostics;
using Newtonsoft.Json;
using Wanderer.Factories;

namespace Wanderer.Compilation
{
    public abstract class Code
    {
        public string Script { get; set; }

        public LuaFactory Factory {get;set;} = new LuaFactory();

        public static Stopwatch Stopwatch = new Stopwatch();

        [JsonConstructor]
        public Code()
        {
        }

        public Code(string script)
        {
            Script = script;
        }
        
        public override string ToString()
        {
            return Script ;
        }
    }
}
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

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

        
        public override string ToString()
        {
            return CsharpCode ;
        }
    }
}
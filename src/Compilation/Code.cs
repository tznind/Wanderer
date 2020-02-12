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
                    "Wanderer.Actors",
                    "Wanderer.Adjectives");
        }

        
        public override string ToString()
        {
            return CsharpCode ;
        }
    }
}
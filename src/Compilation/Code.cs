using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace StarshipWanderer.Compilation
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
                    "StarshipWanderer.Stats", 
                    "System",
                    "StarshipWanderer.Actors",
                    "StarshipWanderer.Adjectives");
        }

        
        public override string ToString()
        {
            return CsharpCode ;
        }
    }
}
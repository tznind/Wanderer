using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using StarshipWanderer.Effects;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Conditions
{
    public interface ICondition
    {
        string CsharpCode { get; set; }
        
        bool IsMet(object o);
    }
    
    public class Code : ICondition, IEffect
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

        public bool IsMet(object forObject)
        {
            return CSharpScript.EvaluateAsync<bool>(CsharpCode, 
                GetScriptOptions()
                , forObject).Result;
        }

        private ScriptOptions GetScriptOptions()
        {
            return ScriptOptions.Default
                .WithReferences(typeof(ICondition).Assembly)
                .WithImports(
                    "StarshipWanderer.Stats", 
                    "System",
                    "StarshipWanderer.Actors",
                    "StarshipWanderer.Adjectives");
        }


        public void Apply(SystemArgs args)
        {
            CSharpScript.EvaluateAsync(CsharpCode, GetScriptOptions(), args).Wait(); 
        }

        public override string ToString()
        {
            return CsharpCode ;
        }
    }
}
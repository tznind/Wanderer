using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class EffectCode : Code, IEffect
    {
        private Script _script;

        public EffectCode(string csharpCode):base(csharpCode)
        {
            _script = CSharpScript.Create(csharpCode, GetScriptOptions(),typeof(SystemArgs));
        }
        public void Apply(SystemArgs args)
        {
            var result = _script.RunAsync(args).Result;

            if (result.Exception != null)
                throw result.Exception;
        }
    }
}
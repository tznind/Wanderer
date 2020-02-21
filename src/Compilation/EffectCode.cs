using System;
using System.Linq;
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
            try
            {
                _script.RunAsync(args).Wait();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{CsharpCode}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
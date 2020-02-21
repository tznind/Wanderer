using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class FrameSourceCode : Code, IFrameSource
    {
        private Script<Frame> _script;

        public FrameSourceCode(string script):base(script)
        {
            _script = CSharpScript.Create<Frame>(script, GetScriptOptions(),typeof(SystemArgs));
        }
        public Frame GetFrame(SystemArgs args)
        {
            try
            {
                return _script.RunAsync(args).Result.ReturnValue;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
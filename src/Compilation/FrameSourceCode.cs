using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class FrameSourceCode : Code, IFrameSource
    {
        private Script<Frame> _script;

        public FrameSourceCode(string csharpCode):base(csharpCode)
        {
            try
            {
                _script = CSharpScript.Create<Frame>(csharpCode, GetScriptOptions(),typeof(SystemArgs));
            }
            catch(Exception ex)
            {

                throw new Exception($"Error compiling '{GetType().Name}' script code '{csharpCode}'",ex);
            }
        }
        public Frame GetFrame(SystemArgs args)
        {

            try
            {
                var result = _script.RunAsync(args).Result;

                if (result.Exception != null)
                    throw result.Exception;

                return result.ReturnValue;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{CsharpCode}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
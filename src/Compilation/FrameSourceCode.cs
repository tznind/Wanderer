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
            _script = CSharpScript.Create<Frame>(csharpCode, GetScriptOptions(),typeof(SystemArgs));
        }
        public Frame GetFrame(SystemArgs args)
        {
            var result = _script.RunAsync(args).Result;

            if (result.Exception != null)
                throw result.Exception;

            return result.ReturnValue;
        }
    }
}
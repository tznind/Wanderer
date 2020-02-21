using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Wanderer.Compilation
{
    public class ConditionCode<T> : Code, ICondition<T>
    {
        private Script<bool> _script;

        public ConditionCode(string script):base(script)
        {
            _script = CSharpScript.Create<bool>(script, GetScriptOptions(),typeof(T));
        }
        public bool IsMet(T forObject)
        {
            try
            {
                return _script.RunAsync(forObject).Result.ReturnValue;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  T was typeof({typeof(T)}) and had value '{forObject}'",ex);
            }
        }

        public bool IsMet(object o)
        {
            return IsMet((T) o);
        }
    }
}
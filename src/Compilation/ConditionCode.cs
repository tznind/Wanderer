using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Wanderer.Compilation
{
    public class ConditionCode<T> : Code, ICondition<T>
    {
        private Script<bool> _script;

        public ConditionCode(string csharpCode):base(csharpCode)
        {
            try
            {
                _script = CSharpScript.Create<bool>(csharpCode, GetScriptOptions(),typeof(T));
            }
            catch(Exception ex)
            {
                
                throw new Exception($"Error compiling '{GetType().Name}' script code '{csharpCode}'",ex);
            }
        }
        public bool IsMet(T forObject)
        {
            try
            {
                var result = _script.RunAsync(forObject).Result;

                if (result.Exception != null)
                    throw result.Exception;

                return result.ReturnValue;

            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{CsharpCode}'.  T was typeof({typeof(T)}) and had value '{forObject}'",ex);
            }
        }

        public bool IsMet(object o)
        {
            return IsMet((T) o);
        }
    }
}
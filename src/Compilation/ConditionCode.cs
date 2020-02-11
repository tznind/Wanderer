using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace StarshipWanderer.Compilation
{
    public class ConditionCode<T> : Code, ICondition<T>
    {
        private Script<bool> _script;

        public ConditionCode(string csharpCode):base(csharpCode)
        {
            _script = CSharpScript.Create<bool>(csharpCode, GetScriptOptions(),typeof(T));
        }
        public bool IsMet(T forObject)
        {
            var result = _script.RunAsync(forObject).Result;

            if (result.Exception != null)
                throw result.Exception;

            return result.ReturnValue;
        }

        public bool IsMet(object o)
        {
            return IsMet((T) o);
        }
    }
}
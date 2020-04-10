using System;
using NLua.Exceptions;

namespace Wanderer.Compilation
{
    public class ConditionCode<T> : Code, ICondition<T>
    {

        public ConditionCode(string script):base(script)
        {
        }
        public bool IsMet(IWorld world,T forObject)
        {
            try
            {
                using(var lua = Factory.Create(world,forObject))
                {
                    var result = lua.DoString(Script);

                    if(result == null || result.Length == 0 || result[0] == null)
                    {
                        if(Script.TrimStart().StartsWith("return"))
                            throw new Exception("Script returned null");
                        else
                            throw new Exception("Script returned null, possibly you are missing starting keyword 'return' on your Condition?");
                    }

                    return (bool)result[0];
                }
            }
            catch(LuaScriptException ex)
            {
                if(ex.IsNetException)
                    throw new Exception(GetThrowMsg(forObject),ex.GetBaseException());
                
                throw new Exception(GetThrowMsg(forObject),ex);
            }
            catch(Exception ex)
            {
                throw new Exception(GetThrowMsg(forObject),ex);
            }
        }

        private string GetThrowMsg(T forObject)
        {
            return $"Error executing '{GetType().Name}' script code '{Script}'.  Arg was Type '{typeof(T).Name}' and had value '{forObject}'";
        }
    }
}
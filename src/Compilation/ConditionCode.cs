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
                    return (bool)lua.DoString(Script)[0];
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
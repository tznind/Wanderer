using System;
using NLua.Exceptions;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class ConditionCode : Code, ICondition
    {

        public ConditionCode(string script):base(script)
        {
            Script = Script.TrimStart();

            if (!Script.StartsWith("return"))
                Script = "return " + Script;
        }
        public bool IsMet(IWorld world,SystemArgs forObject)
        {
            if(forObject == null)
                throw new ArgumentNullException(nameof(forObject));

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

        private string GetThrowMsg(SystemArgs forObject)
        {
            return $"Error executing '{GetType().Name}' script code '{Script}'.  Arg was Type '{forObject.GetType().Name}' and had value '{forObject}'";
        }
    }
}
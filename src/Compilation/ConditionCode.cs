using System;

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
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  T was typeof({typeof(T)}) and had value '{forObject}'",ex);
            }
        }

        public bool IsMet(IWorld world,object o)
        {
            return IsMet(world,(T) o);
        }
    }
}
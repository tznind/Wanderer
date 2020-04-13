using System;
using System.Runtime.Serialization;
using NLua.Exceptions;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class EffectCode : Code, IEffect
    {

        public EffectCode(string script):base(script)
        {
        }
        public void Apply(SystemArgs args)
        {
            try
            {
                using (var lua = Factory.Create(args.World, args))
                {
                    Stopwatch.Start();
                    lua.DoString(Script);
                }

            }
            catch (LuaScriptException ex)
            {
                if (ex.IsNetException)
                    throw new Exception(GetThrowMsg(args), ex.GetBaseException());

                throw new Exception(GetThrowMsg(args), ex);
            }
            catch (Exception ex)
            {
                throw new Exception(GetThrowMsg(args), ex);
            }
            finally
            {
                Stopwatch.Stop();
            }
        }

        private string GetThrowMsg(SystemArgs args)
        {
            return $"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'";
        }
    }
}
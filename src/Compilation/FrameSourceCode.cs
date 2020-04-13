using System;
using NLua.Exceptions;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class FrameSourceCode : Code, IFrameSource
    {
        public FrameSourceCode(string script):base(script)
        {
        }
        public Frame GetFrame(SystemArgs args)
        {
            try
            {
                using (var lua = Factory.Create(args.World, args))
                {
                    Stopwatch.Start();
                    return (Frame) lua.DoString(Script)[0];
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
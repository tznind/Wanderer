using System;
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
                return (Frame)GetLua(args.World,args).DoString(Script)[0];
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
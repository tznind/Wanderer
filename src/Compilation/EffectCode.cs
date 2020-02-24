using System;
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
                
                using(var lua = Factory.Create(args.World,args))
                    lua.DoString(Script);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
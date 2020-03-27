using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class ActionTargetCode : Code, IActionTarget
    {
        public ActionTargetCode(string script):base(script)
        {
        }
        public IEnumerable<IHasStats> Get(SystemArgs args)
        {
            try
            {
                using(var lua = Factory.Create(args.World,args))
                    return (IEnumerable<IHasStats>)lua.DoString(Script)[0];
            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'",ex);
            }
        }
    }
}
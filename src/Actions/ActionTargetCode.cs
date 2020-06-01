using System;
using System.Collections.Generic;
using NLua.Exceptions;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    /// <summary>
    /// Describes how to pick eligible targets for a given <see cref="IAction"/>.  This is primarily used for defining targeting criteria of a custom <see cref="ActionBlueprint"/>
    /// </summary>
    public class ActionTargetCode : Code, IActionTarget
    {
        /// <summary>
        /// Creates a new targeting criteria using the provided lua code.
        /// </summary>
        /// <param name="script"></param>
        public ActionTargetCode(string script):base(script)
        {
        }

        /// <inheritdoc />
        public IEnumerable<IHasStats> Get(SystemArgs args)
        {
            try
            {
                using(var lua = Factory.Create(args.World,args))
                    return (IEnumerable<IHasStats>)lua.DoString(Script)[0];
            }
            catch(LuaScriptException ex)
            {
                if(ex.IsNetException)
                    throw new Exception(GetThrowMsg(args),ex.GetBaseException());
                
                throw new Exception(GetThrowMsg(args),ex);
            }
            catch(Exception ex)
            {
                throw new Exception(GetThrowMsg(args),ex);
            }
        }

        private string GetThrowMsg(SystemArgs args)
        {
            return $"Error executing '{GetType().Name}' script code '{Script}'.  SystemArgs were for '{args.Recipient}'";
        }
    }
}
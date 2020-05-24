using System;
using System.Collections.Generic;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    /// <summary>
    /// Picks viable targets for an <see cref="IAction"/> by invoking a provided Func
    /// </summary>
    class FuncTarget: IActionTarget
    {
        private readonly Func<SystemArgs, IEnumerable<IHasStats>> _func;

        public FuncTarget(Func<SystemArgs,IEnumerable<IHasStats>> func)
        {
            _func = func;
        }
        public IEnumerable<IHasStats> Get(SystemArgs args)
        {
            return _func(args);
        }
    }
}
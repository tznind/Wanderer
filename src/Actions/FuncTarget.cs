using System;
using System.Collections.Generic;
using Wanderer.Systems;

namespace Wanderer.Actions
{
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
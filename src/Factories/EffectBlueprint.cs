using System;
using System.Collections.Generic;
using Wanderer.Compilation;

namespace Wanderer.Factories
{
    public class EffectBlueprint
    {
        public string Lua {get; set;}

        public IEnumerable<IEffect> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new EffectCode(Lua);
        }
    }
}
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public class Giant : Adjective
    {
        public Giant(IHasStats owner) : base(owner)
        {
            IsPrefix = true;
            BaseStats[Stat.Fight] = 30;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public class Tired : Adjective
    {
        public Tired(IHasStats owner) : base(owner)
        {
            BaseStats[Stat.Fight] = -10;
        }
        
        public override IEnumerable<string> GetDescription()
        {
            yield return "Reduces Stats";
        }
    }
}

using System.Collections.Generic;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public class Strong : Adjective
    {
        public Strong(IHasStats owner):base(owner)
        {
            BaseStats[Stat.Fight] = 10;
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Improves Fight";
        }
    }
}
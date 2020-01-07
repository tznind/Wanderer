using System.Collections.Generic;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
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
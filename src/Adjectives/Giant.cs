using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Giant : Adjective
    {
        public Giant(IHasStats owner) : base(owner)
        {
            IsPrefix = true;
            BaseStats[Stat.Fight] = 30;
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Fight better";
            
            if (Owner is IActor)
                yield return "Harder to heal";
        }
    }
}
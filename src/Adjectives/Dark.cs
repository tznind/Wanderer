using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Dark : Adjective
    {
        public Dark(IPlace owner) : base(owner)
        {
            BaseStats[Stat.Fight] = -10;
        }
    }
}

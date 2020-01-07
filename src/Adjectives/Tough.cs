using System.Collections.Generic;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Adjectives
{
    public class Tough : Adjective
    {
        public Tough(IHasStats owner) : base(owner)
        {
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Wounds do not get infected";
        }
    }
}
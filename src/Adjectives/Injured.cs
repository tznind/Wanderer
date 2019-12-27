using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Injured : Adjective
    {
        public Injured(IActor actor):base(actor)
        {
            Modifiers[Stat.Fight] = -10;
        }

    }
}
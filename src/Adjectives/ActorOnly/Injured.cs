using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives.ActorOnly
{
    public class Injured : Adjective
    {
        public Injured(IActor actor):base(actor)
        {
            IsPrefix = false;
            BaseStats[Stat.Fight] = -10;
        }

    }
}
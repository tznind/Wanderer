using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives.RoomOnly
{
    public class Dark : Adjective
    {
        public Dark(IPlace owner) : base(owner)
        {
            BaseStats[Stat.Fight] = -10;
        }
    }
}

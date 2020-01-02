using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Strong : Adjective
    {
        public Strong(IHasStats owner):base(owner)
        {
            BaseStats[Stat.Fight] = 10;
        }
    }
}
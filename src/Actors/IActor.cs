using NStack;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public interface IActor
    {
        string Name { get; set; }
        StatsCollection BaseStats { get; }



    }
}

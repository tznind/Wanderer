using System.Collections.Generic;
using NStack;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public interface IActor
    {
        string Name { get; set; }
        StatsCollection BaseStats { get; set; }

        IEnumerable<IBehaviour> GetFinalBehaviours();
        
        StatsCollection GetFinalStats();
    }
}

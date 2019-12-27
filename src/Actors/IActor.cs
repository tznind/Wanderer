using System.Collections.Generic;
using NStack;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public interface IActor
    {
        string Name { get; set; }

        HashSet<IAdjective> Adjectives { get; set; }

        StatsCollection BaseStats { get; set; }

        IEnumerable<IBehaviour> GetFinalBehaviours();
        
        StatsCollection GetFinalStats();
    }
}

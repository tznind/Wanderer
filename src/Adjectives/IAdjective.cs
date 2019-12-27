using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjective
    {
        IActor Owner { get; set; }

        StatsCollection Modifiers { get; }
        List<IBehaviour> Behaviours { get; }
        bool IsActive();
    }
}

using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjective : IHasStats
    {
        bool IsActive();
    }
}

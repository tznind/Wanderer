using System.Collections.Generic;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjectiveFactory
    {
        IEnumerable<IAdjective> GetAvailableAdjectives(IActor actor);

    }
}
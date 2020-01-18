using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjectiveFactory
    {
        IEnumerable<IAdjective> GetAvailableAdjectives(IPlace place);
        IEnumerable<IAdjective> GetAvailableAdjectives(IActor actor);
        IEnumerable<IAdjective> GetAvailableAdjectives(IItem item);
    }
}
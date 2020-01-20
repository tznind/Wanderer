using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjectiveFactory
    {
        IEnumerable<IAdjective> GetAvailableAdjectives<T>(T place) where T: IHasStats;
    }
}
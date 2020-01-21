using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Factories.Blueprints;

namespace StarshipWanderer.Factories
{
    public interface IAdjectiveFactory
    {
        IEnumerable<IAdjective> GetAvailableAdjectives<T>(T place) where T: IHasStats;

        IAdjective Create(IHasStats owner,AdjectiveBlueprint blueprint);

        IAdjective Create(IHasStats s, Type adjectiveType);
    }
}
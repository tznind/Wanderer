using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public interface IAdjectiveFactory
    {
        IEnumerable<IAdjective> GetAvailableAdjectives<T>(T place) where T: IHasStats;

        IAdjective Create(IHasStats owner,AdjectiveBlueprint blueprint);

        IAdjective Create(IHasStats s, Type adjectiveType);
    }
}
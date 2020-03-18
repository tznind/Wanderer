using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public interface IAdjectiveFactory
    {
        List<AdjectiveBlueprint> Blueprints { get; set; }

        IAdjective Create(IWorld world,IHasStats owner,AdjectiveBlueprint blueprint);
        IAdjective Create(IHasStats s, Type adjectiveType);
        IAdjective Create(IWorld world,IHasStats s, Guid guid);

        IAdjective Create(IWorld world,IHasStats s, string name);

        /// <summary>
        /// Adds all <see cref="HasStatsBlueprint.MandatoryAdjectives"/> and
        /// some <see cref="HasStatsBlueprint.OptionalAdjectives"/> to
        /// <paramref name="owner"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="owner"></param>
        /// <param name="ownerBlueprint">the owners blueprint (NOT an <see cref="AdjectiveBlueprint"/>)</param>
        void AddAdjectives(IWorld world,IHasStats owner, HasStatsBlueprint ownerBlueprint);
    }
}
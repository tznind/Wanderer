using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    /// <summary>
    /// Creates <see cref="IAdjective"/> instances based on <see cref="AdjectiveBlueprint"/>.  The resulting instances are always added onto an owner (<see cref="IHasStats"/>) immediately
    /// </summary>
    public interface IAdjectiveFactory: IHasStatsFactory
    {
        /// <summary>
        /// Blueprints for all adjectives which can be created by this factory
        /// </summary>
        List<AdjectiveBlueprint> Blueprints { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="IAdjective"/> and attaches it onto <paramref name="owner"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="owner"></param>
        /// <param name="blueprint"></param>
        /// <returns></returns>
        IAdjective Create(IWorld world,IHasStats owner,AdjectiveBlueprint blueprint);

        /// <summary>
        /// Overload which creates a new instance of the <paramref name="adjectiveType"/> and attaches it to <paramref name="owner"/>.  The Type must have a single argument constructor that takes the owning <see cref="IHasStats"/> instance
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="adjectiveType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="adjectiveType"/> does not have a compatible constructor</exception>
        IAdjective Create(IHasStats owner, Type adjectiveType);

        /// <summary>
        /// Overload that looks up the blueprint to create by <paramref name="guid"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="s"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="GuidNotFoundException"></exception>
        IAdjective Create(IWorld world,IHasStats s, Guid guid);

        /// <summary>
        /// Overload that looks up the blueprint to create by <paramref name="name"/>.  Also supports passing a Type name (when <see cref="Compiler.TypeFactory"/> is configured to include the hosting assembly)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="s"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NamedObjectNotFoundException"></exception>
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
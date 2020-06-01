using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    /// <summary>
    /// Creates instances of <see cref="IAction"/> based on <see cref="ActionBlueprint"/>.  Actions are usually added to an <see cref="IHasStats"/> object e.g. 'Shine around' action could be added to a 'Torch' item
    /// </summary>
    public interface IActionFactory: IHasStatsFactory
    {
        /// <summary>
        /// Action blueprints which get stamped out by this factory
        /// </summary>
        List<ActionBlueprint> Blueprints { get; set; }
        
        /// <summary>
        /// Creates a new <see cref="IAction"/> based on the <paramref name="blueprint"/> and adds it as a <see cref="IHasStats.BaseActions"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="blueprint"></param>
        /// <returns></returns>
        IAction Create(IWorld world, IHasStats onto, ActionBlueprint blueprint);

        /// <summary>
        /// Overload that takes the name of a <see cref="ActionBlueprint"/> (which must exist)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NamedObjectNotFoundException"></exception>
        IAction Create(IWorld world, IHasStats onto, string name);

        /// <summary>
        /// Overload that takes the <see cref="HasStatsBlueprint.Identifier"/>  (which must exist)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        /// <exception cref="GuidNotFoundException"></exception>
        IAction Create(IWorld world, IHasStats onto, Guid g);
    }
}
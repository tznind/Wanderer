using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public interface IActionFactory
    {
        /// <summary>
        /// Action blueprints which get stamped out by this factory
        /// </summary>
        List<ActionBlueprint> Blueprints { get; set; }

        IAction Create(IWorld world, IHasStats onto, ActionBlueprint blueprint);
    }
}
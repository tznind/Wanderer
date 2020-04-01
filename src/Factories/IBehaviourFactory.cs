using System;
using System.Collections.Generic;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public interface IBehaviourFactory
    {

        /// <summary>
        /// Behaviour blueprints which get stamped out by this factory
        /// </summary>
        List<BehaviourBlueprint> Blueprints { get; set; }

        IBehaviour Create(IWorld world, IHasStats onto, BehaviourBlueprint blueprint);
        IBehaviour Create(IWorld world, IHasStats onto, string name);
        IBehaviour Create(IWorld world, IHasStats onto, Guid g);
    }
}
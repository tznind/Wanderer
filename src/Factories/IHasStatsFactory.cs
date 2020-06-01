using System;
using System.Collections.Generic;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    /// <summary>
    /// Base interface for all factories which create <see cref="IHasStats"/> instances based on <see cref="HasStatsBlueprint"/> derived blueprints
    /// </summary>
    public interface IHasStatsFactory
    {
        /// <summary>
        /// List of all the unique blueprints spawned so far by this factory
        /// </summary>
        HashSet<Guid> UniquesSpawned { get; set; }
    
        /// <summary>
        /// Behaviours which should be added to all instances stamped out by this factory (unless they list <see cref="HasStatsBlueprint.SkipDefaultBehaviours"/>)
        /// </summary>
        List<BehaviourBlueprint> DefaultBehaviours { get; set; }

        /// <summary>
        /// Actions which should be added to all instances stamped out by this factory (unless they list <see cref="HasStatsBlueprint.SkipDefaultActions"/>)
        /// </summary>
        List<ActionBlueprint> DefaultActions { get; set; }
    }
}
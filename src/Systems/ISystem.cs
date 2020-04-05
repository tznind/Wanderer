using System;
using System.Collections.Generic;
using Wanderer.Actors;

namespace Wanderer.Systems
{
    /// <summary>
    /// A system for applying a range of possibly cumulative effects on an
    /// <see cref="IActor"/> e.g. injuries
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Human readable name for the system
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The unique identifier for this system so that it can be referenced from scripts
        /// etc.  This should be a constant (Don't use NewGuid!).  When sub-classing it is
        /// permissible to use the parents guid if you are semantically the same (e.g. subclass
        /// methods are alternate ways to load the system)
        /// </summary>
        Guid Identifier { get; set; }

        /// <summary>
        /// Apply the system to the recipient
        /// </summary>
        void Apply(SystemArgs args);

        /// <summary>
        /// Apply the system once to each of the <paramref name="recipients"/>
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="args"></param>
        void ApplyToAll(IEnumerable<IHasStats> recipients, SystemArgs args);
    }
}

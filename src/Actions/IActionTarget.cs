using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    /// <summary>
    /// Function for fetching available targets for a custom <see cref="IAction"/>
    /// </summary>
    public interface IActionTarget
    {
        /// <summary>
        /// Returns all viable targets based on the current game state as described by <paramref name="args"/>
        /// </summary>
        /// <param name="args">Arguments where <see cref="SystemArgs.AggressorIfAny"/> is the one performing the action and <see cref="SystemArgs.Recipient"/> is the one granting the action (may be the same or may be an item, room etc)</param>
        /// <returns></returns>
         IEnumerable<IHasStats> Get(SystemArgs args);
    }
}
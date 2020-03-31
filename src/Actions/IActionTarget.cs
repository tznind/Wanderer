using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public interface IActionTarget
    {
         IEnumerable<IHasStats> Get(SystemArgs args);
    }
}
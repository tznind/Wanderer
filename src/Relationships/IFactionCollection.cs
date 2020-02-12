using System;
using System.Collections.Generic;
using Wanderer.Actors;

namespace Wanderer.Relationships
{
    public interface IFactionCollection : IList<IFaction>
    {
        /// <summary>
        /// Return a random faction with the given role (or null if none fit the bill)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="withRole">Pass to restrict the picked faction or omit to pick
        /// from any</param>
        /// <returns></returns>
        IFaction GetRandomFaction(Random r,params FactionRole[] withRole);
    }
}
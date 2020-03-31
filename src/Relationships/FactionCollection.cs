using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;

namespace Wanderer.Relationships
{
    public class FList<IAction> : List<IFaction>,IFList<IAction>
    {
        public IFaction GetRandomFaction(Random r, params FactionRole[] withRole)
        {
            var suitable = this.Where(f => withRole.Length == 0 || withRole.Contains(f.Role)).ToList();
            if (suitable.Any())
                return suitable[r.Next(suitable.Count)];

            return null;
        }
    }
}
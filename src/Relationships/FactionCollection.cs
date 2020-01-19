using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public class FactionCollection : List<IFaction>,IFactionCollection
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
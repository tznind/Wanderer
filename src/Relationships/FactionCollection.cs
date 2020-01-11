using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public class FactionCollection : List<IFaction>,IFactionCollection
    {
        public void AssignFactions(IActor actor)
        {
            if (this.Any())
            {
                var faction = this[actor.CurrentLocation.World.R.Next(this.Count)];
                actor.FactionMembership.Add(faction);
            }
        }
    }
}
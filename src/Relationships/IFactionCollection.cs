using System.Collections.Generic;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public interface IFactionCollection : IList<IFaction>
    {
        void AssignFactions(IActor actor);

    }
}
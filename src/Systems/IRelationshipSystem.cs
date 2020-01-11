using System.Collections.Generic;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Systems
{
    public interface IRelationshipSystem : IList<IRelationship>, ISystem
    {

    }
}
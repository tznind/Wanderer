using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Systems
{
    public interface IRelationshipSystem : IList<IRelationship>, ISystem
    {
        /// <summary>
        /// Returns the total attitude in all relationships that apply when
        /// the <paramref name="observer"/> considers how he feels about the
        /// <param name="observed"></param>
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="observed"></param>
        /// <returns></returns>
        double SumBetween(IActor observer, IActor observed);
    }
}
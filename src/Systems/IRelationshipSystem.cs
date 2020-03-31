using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Wanderer.Systems
{
    public interface IRelationshipSystem : IList<IRelationship>, ISystem
    {
        /// <summary>
        /// Returns the total attitude in all relationships that apply when
        /// the <paramref name="observer"/> considers how he feels about the
        /// <paramref name="observed"/>
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="observed"></param>
        /// <returns></returns>
        double SumBetween(IActor observer, IActor observed);
    }
}
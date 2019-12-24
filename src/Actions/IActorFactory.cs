using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public interface IActorFactory
    {
        IEnumerable<IActor> Create(IPlace place);
    }
}
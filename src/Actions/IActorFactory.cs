using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public interface IActorFactory
    {
        void Create(IWorld world, IPlace place);
    }
}
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer.Items
{
    public interface IItemFactory
    {
        IItem Create(IPlace inPlace);
        IItem Create(IActor forActor);

    }
}
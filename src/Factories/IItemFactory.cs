using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IItemFactory
    {
        IItem Create(IPlace inPlace);
        IItem Create(IActor forActor);

    }
}
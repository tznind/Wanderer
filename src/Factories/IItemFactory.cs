using StarshipWanderer.Actors;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IItemFactory
    {
        ItemBlueprint[] Blueprints { get; set; }
        IItem Create(IWorld world, ItemBlueprint blueprint);

    }
}
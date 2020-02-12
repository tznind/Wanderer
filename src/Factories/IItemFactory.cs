using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Places;

namespace Wanderer.Factories
{
    public interface IItemFactory
    {
        ItemBlueprint[] Blueprints { get; set; }
        IItem Create(IWorld world, ItemBlueprint blueprint);

    }
}
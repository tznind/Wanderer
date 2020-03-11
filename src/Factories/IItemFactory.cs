using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    public interface IItemFactory
    {
        List<ItemBlueprint> Blueprints { get; set; }
        IItem Create(IWorld world, ItemBlueprint blueprint);

    }
}
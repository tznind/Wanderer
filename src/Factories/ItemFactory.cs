using System;
using System.Linq;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    public class ItemFactory : HasStatsFactory<ItemBlueprint,IItem>,IItemFactory
    {
        public IItem Create(IWorld world,ItemBlueprint blueprint)
        {
            var item = 
                blueprint.Stack.HasValue ?
                new ItemStack(blueprint.Name,blueprint.Stack.Value):
                new Item(blueprint.Name);

            AddBasicProperties(world,item,blueprint,"read");
            world.AdjectiveFactory.AddAdjectives(world,item, blueprint);

            if(blueprint.Require != null)
                item.Require = blueprint.Require;

            if (blueprint.Slot != null)
                item.Slot = blueprint.Slot;

            return item;
        }
        
        public IItem Create(IWorld world, Guid guid)
        {
            return Create(world, GetBlueprint(guid));
        }

        public IItem Create(IWorld world, string name)
        {
            return Create(world, GetBlueprint(name));
        }
    }
}
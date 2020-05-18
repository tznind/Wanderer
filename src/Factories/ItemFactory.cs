using System;
using System.Linq;
using Wanderer.Compilation;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    public class ItemFactory : HasStatsFactory<ItemBlueprint,IItem>,IItemFactory
    {
        public IItem Create(IWorld world,ItemBlueprint blueprint)
        {
            HandleInheritance(blueprint);

            var item = 
                blueprint.Stack.HasValue ?
                new ItemStack(blueprint.Name,blueprint.Stack.Value):
                new Item(blueprint.Name);

            AddBasicProperties(world,item,blueprint,"read");
            world.AdjectiveFactory.AddAdjectives(world,item, blueprint);

            if(blueprint.Require != null)
                foreach (var conditionBlueprint in blueprint.Require)
                {
                    if(!string.IsNullOrWhiteSpace(conditionBlueprint.Lua))
                        item.Require.Add(new ConditionCode<IHasStats>(conditionBlueprint.Lua));

                    //TODO: build other conditions here
                }

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

        public void Create(IWorld world, Room room, IFaction faction, RoomBlueprint roomBlueprintIfAny)
        {
            var max = roomBlueprintIfAny?.OptionalItemsMax ?? 5;

            if(max <= 0)
                return;

            var min = roomBlueprintIfAny?.OptionalItemsMin??1;

            if(min > max)
                throw new ArgumentOutOfRangeException($"OptionalItemsMin should be lower than OptionalItemsMax for room blueprint '{roomBlueprintIfAny}'");

            int numberOfItems = world.R.Next(min,max+1);

            var pickFrom = world.ItemFactory.Blueprints.Where(b => b.SuitsFaction(faction)).ToList();

            if (roomBlueprintIfAny?.OptionalItems != null)
                pickFrom = pickFrom.Union(roomBlueprintIfAny.OptionalItems).ToList();


            //create some random items
            for (int i = 0; i < numberOfItems; i++)
            {
                var item = pickFrom.GetRandom(world.R);

                //could be no blueprints
                if(item != null)
                    room.SpawnItem(item);
            }
        }
    }
}
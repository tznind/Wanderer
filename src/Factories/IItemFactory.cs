using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    public interface IItemFactory: IHasStatsFactory
    {
        List<ItemBlueprint> Blueprints { get; set; }
        IItem Create(IWorld world, ItemBlueprint blueprint);

        IItem Create(IWorld world, Guid blueprint);
        IItem Create(IWorld world, string name);

        /// <summary>
        /// Create a selection of random items fitting the faction / room blueprint
        /// </summary>
                /// <param name="world"></param>
        /// <param name="room">Where to create the items</param>
        /// <param name="faction"></param>
        /// <param name="roomBlueprintIfAny">Optional blueprint which might contain thematic room items etc</param>
        void Create(IWorld world, Room room, IFaction faction, RoomBlueprint roomBlueprintIfAny);
    }
}
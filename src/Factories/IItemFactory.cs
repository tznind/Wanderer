using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    /// <summary>
    /// Creates <see cref="IItem"/> and <see cref="IItemStack"/> instances based on <see cref="ItemBlueprint"/>
    /// </summary>
    public interface IItemFactory: IHasStatsFactory
    {
        /// <summary>
        /// Item blueprints which can be created by this factory.  Note that this is not all items in the world, unique <see cref="ItemBlueprint"/> can also be found under e.g. <see cref="ActorBlueprint.MandatoryItems"/> which would not be listed here (see <see cref="IWorld.TryGetBlueprint(string)"/>
        /// </summary>
        List<ItemBlueprint> Blueprints { get; set; }

        /// <summary>
        /// Creates a new <see cref="IItem"/> or <see cref="IItemStack"/> based on the <paramref name="blueprint"/>.  This item will not yet exist in any room / actors inventory
        /// </summary>
        /// <param name="world"></param>
        /// <param name="blueprint"></param>
        /// <returns></returns>
        IItem Create(IWorld world, ItemBlueprint blueprint);

        /// <summary>
        /// Overload that looks up the blueprint by <see cref="HasStatsBlueprint.Identifier"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="blueprint"></param>
        /// <returns></returns>
        /// <exception cref="GuidNotFoundException"></exception>
        IItem Create(IWorld world, Guid blueprint);

        /// <summary>
        /// Overload that looks up the blueprint by <see cref="HasStatsBlueprint.Name"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NamedObjectNotFoundException"></exception>
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
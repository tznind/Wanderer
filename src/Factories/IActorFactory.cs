using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Relationships;
using System.Collections.Generic;

namespace Wanderer.Factories
{
    /// <summary>
    /// Factory for creating <see cref="IActor"/> instances based on <see cref="ActorBlueprint"/>.  Typically these are <see cref="Npc"/> (i.e. not the player)
    /// </summary>
    public interface IActorFactory : IHasStatsFactory
    {
        /// <summary>
        /// Npc blueprints which get stamped out by this factory
        /// </summary>
        List<ActorBlueprint> Blueprints { get; set; }
        

        /// <summary>
        /// Slots (1 head 2 arms etc) that Actors get when created (unless blueprint
        /// specifically says otherwise)
        /// </summary>
        SlotCollection DefaultSlots { get; set; }
        
        /// <summary>
        /// Create some npcs in the room
        /// </summary>
        /// <param name="world"></param>
        /// <param name="room">Where to create the actor</param>
        /// <param name="faction"></param>
        /// <param name="roomBlueprintIfAny">Optional blueprint which might contain thematic room items etc</param>
        void Create(IWorld world, IRoom room, IFaction faction, RoomBlueprint roomBlueprintIfAny);

        /// <summary>
        /// Create actor new resident in <paramref name="room"/> by stamping out the actor <paramref name="blueprint"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="room">Where to create the actor</param>
        /// <param name="faction"></param>
        /// <param name="blueprint">Description of the npc to create including optional bits</param>
        /// <param name="roomBlueprintIfAny">Optional blueprint which might contain thematic room items etc</param>
        /// <returns></returns>
        IActor Create(IWorld world, IRoom room, IFaction faction, ActorBlueprint blueprint, RoomBlueprint roomBlueprintIfAny);
    }
}
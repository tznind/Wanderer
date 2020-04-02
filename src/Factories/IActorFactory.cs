using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Relationships;
using System.Collections.Generic;

namespace Wanderer.Factories
{
    public interface IActorFactory
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
        /// Default behaviour blueprints that get stamped onto every actor produced
        /// by the factory.
        /// </summary>
        List<BehaviourBlueprint> DefaultBehaviours { get; set; }

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

        /// <summary>
        /// Adds all <see cref="DefaultBehaviours"/> onto <paramref name="actor"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="actor"></param>
        void AddDefaultBehaviours(IWorld world, IActor actor);
    }
}
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Places;
using Wanderer.Relationships;

namespace Wanderer.Factories
{
    public interface IActorFactory
    {
        /// <summary>
        /// Npc blueprints which get stamped out by this factory
        /// </summary>
        ActorBlueprint[] Blueprints { get; set; }
        
        /// <summary>
        /// Factory for items which fit the actors theme of this factory
        /// </summary>
        IItemFactory ItemFactory { get; set; }

        /// <summary>
        /// Create some npcs in the room
        /// </summary>
        /// <param name="world"></param>
        /// <param name="place">Where to create the actor</param>
        /// <param name="faction"></param>
        /// <param name="roomBlueprintIfAny">Optional blueprint which might contain thematic room items etc</param>
        void Create(IWorld world, IPlace place, IFaction faction, RoomBlueprint roomBlueprintIfAny);

        /// <summary>
        /// Create a new resident in <paramref name="place"/> by stamping out the actor <paramref name="blueprint"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="place">Where to create the actor</param>
        /// <param name="faction"></param>
        /// <param name="blueprint">Description of the npc to create including optional bits</param>
        /// <param name="roomBlueprintIfAny">Optional blueprint which might contain thematic room items etc</param>
        /// <returns></returns>
        IActor Create(IWorld world, IPlace place, IFaction faction, ActorBlueprint blueprint, RoomBlueprint roomBlueprintIfAny);
    }
}
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Factories
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
        /// <param name="place"></param>
        /// <param name="faction"></param>
        /// <param name="blueprint"></param>
        void Create(IWorld world, IPlace place, IFaction faction, RoomBlueprint blueprint);
    }
}
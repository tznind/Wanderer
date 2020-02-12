using System;
using Wanderer.Relationships;

namespace Wanderer.Factories.Blueprints
{
    public class RoomBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// True to make this room the first room to spawn in the world.
        /// </summary>
        public bool StartingRoom { get; set; } = false;

        /// <summary>
        /// Null if the room thematically fits any faction, otherwise the <see cref="IFaction.Identifier"/>
        /// </summary>
        public Guid? Faction { get; set; }

        /// <summary>
        /// The digit to render in maps for this room
        /// </summary>
        public char Tile { get; set; }

        /// <summary>
        /// Actors that should definitely be in the room
        /// </summary>
        public ActorBlueprint[] MandatoryActors { get; set; } = new ActorBlueprint[0];
        
        /// <summary>
        /// Special thematic actors that fit the room that random actors can be chosen from
        /// </summary>
        public ActorBlueprint[] OptionalActors { get; set; } = new ActorBlueprint[0];

        
        /// <summary>
        /// MandatoryItems that should definitely be in the room
        /// </summary>
        public ItemBlueprint[] MandatoryItems { get; set; } = new ItemBlueprint[0];
        
        /// <summary>
        /// Special thematic actors that fit the room that random actors can be chosen from
        /// </summary>
        public ItemBlueprint[] OptionalItems { get; set; } = new ItemBlueprint[0];



    }
}
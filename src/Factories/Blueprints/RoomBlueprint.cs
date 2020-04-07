using System;
using Wanderer.Relationships;

namespace Wanderer.Factories.Blueprints
{
    public class RoomBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Makes the room only appear in the given location
        /// </summary>
        public Point3 FixedLocation { get; set; }
        
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
        /// The maximum number of <see cref="OptionalActors"/> to spawn
        /// </summary>
        public int? OptionalActorsMax {get;set;}

        /// <summary>
        /// Leave null for the default (horizontal movement).  Specify explicit directions to allow
        /// only those directions of movement out of the room
        /// </summary>
        public Direction[] LeaveDirections { get; set; }
        
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
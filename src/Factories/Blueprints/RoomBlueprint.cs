using System;
using System.Linq;
using Wanderer.Relationships;
using Wanderer.Rooms;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to create instances of <see cref="IRoom"/>
    /// </summary>
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
        /// The minimum number of <see cref="OptionalActors"/> to spawn
        /// </summary>
        public int? OptionalActorsMin {get;set;}

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


        /// <summary>
        /// The maximum number of <see cref="OptionalItems"/> to spawn
        /// </summary>
        public int? OptionalItemsMax {get;set;}


        /// <summary>
        /// The minimum number of <see cref="OptionalItems"/> to spawn
        /// </summary>
        public int? OptionalItemsMin {get;set;}

        /// <summary>
        /// Returns the named blueprint if it is this one or exists amongst the <see cref="MandatoryActors"/>, <see cref="MandatoryItems"/> etc
        /// </summary>
        public override HasStatsBlueprint TryGetBlueprint(string name)
        {
            return base.TryGetBlueprint(name) ??
            MandatoryActors.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null) ??
            MandatoryItems.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null) ??
            OptionalActors.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null)??
            (HasStatsBlueprint)OptionalItems.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null);
        }
    }
}
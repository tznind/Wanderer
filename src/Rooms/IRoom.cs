using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Relationships;

namespace Wanderer.Rooms
{
    public interface IRoom : IHasStats
    {
        /// <summary>
        /// Who currently controls this place (can be null)
        /// </summary>
        IFaction ControllingFaction { get; set; }

        /// <summary>
        /// True if the player has uncovered this location yet
        /// </summary>
        bool IsExplored { get; set; }

        IWorld World { get; set; }
        
        /// <summary>
        /// The single character to render for this location in maps.
        /// </summary>
        char Tile { get; }
        
        /// <summary>
        /// Items that are not owned by anyone yet
        /// </summary>
        List<IItem> Items { get;set; }

        [JsonIgnore]
        IEnumerable<IActor> Actors { get; }

        Point3 GetPoint();
        
        /// <summary>
        /// Determines which directions you can leave the room in
        /// </summary>
        HashSet<Direction> LeaveDirections { get; set; }
        
        /// <summary>
        /// Spawn a new item in the <see cref="IRoom"/>
        /// </summary>
        /// <param name="blue"></param>
        /// <returns></returns>
        IItem SpawnItem(ItemBlueprint blue);

        
        /// <summary>
        /// Spawn a new item in the <see cref="IRoom"/>
        /// </summary>
        /// <param name="g"></param>
        /// <exception cref="GuidNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(Guid g);
        
        /// <summary>
        /// Spawn a new item in the <see cref="IRoom"/>
        /// </summary>
        /// <param name="name"></param>
        ///  <exception cref="NamedObjectNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(string name);
    }
}
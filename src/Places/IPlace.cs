using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Places
{
    public interface IPlace : IHasStats
    {
        IWorld World { get; set; }
        
        /// <summary>
        /// The single character to render for this location in maps.
        /// </summary>
        char Tile { get; }
        
        /// <summary>
        /// Items that are not owned by anyone yet
        /// </summary>
        HashSet<IItem> Items { get;set; }

        Point3 GetPoint();
    }
}
using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public interface IPlace : IHasStats
    {
        IWorld World { get; set; }
        
        /// <summary>
        /// The single character to render for this location in maps.
        /// </summary>
        char Tile { get; }
        
        Point3 GetPoint();
    }
}
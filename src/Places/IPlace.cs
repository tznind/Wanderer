using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public interface IPlace
    {
        IWorld World { get; set; }

        string Title { get; set; }

        /// <summary>
        /// The single character to render for this location in maps.
        /// </summary>
        char Tile { get; }

        /// <summary>
        /// The actions people can do because they are in this room
        /// </summary>
        IList<IAction> BaseActions { get; set; }

        /// <summary>
        /// Adds a new action which can be performed in the room
        /// </summary>
        /// <param name="action"></param>
        void AddAction(IAction action);
        
        IList<IAction> GetActions(IActor actor);
        Point3 GetPoint();
    }
}
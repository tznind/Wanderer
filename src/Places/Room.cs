using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public class Room : IPlace
    {
        public IWorld World { get; set; }
        public string Title { get; set; }

        public IList<IAction> BaseActions { get; set; } = new List<IAction>();

        /// <inheritdoc/>
        public char Tile { get; set; } = '.';

        public Room(string title,IWorld world)
        {
            Title = title;
            World = world;
        }

        public void AddAction(IAction action)
        {
            BaseActions.Add(action);
        }

        public IList<IAction> GetActions(IActor actor)
        {
            return BaseActions;
        }

        public Point3 GetPoint()
        {
            return World.Map.GetPoint(this);
        }

        public override string ToString()
        {
            return Title ?? "Unnamed Room";
        }
    }
}
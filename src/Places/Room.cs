using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Places
{
    public class Room : IPlace
    {
        public IWorld World { get; set; }
        public string Name { get; set; }

        public HashSet<IAction> BaseActions { get; set; } = new HashSet<IAction>();
        public StatsCollection BaseStats { get; set; } = new StatsCollection();
        public HashSet<IBehaviour> BaseBehaviours { get; set; } = new HashSet<IBehaviour>();

        /// <inheritdoc/>
        public char Tile { get; set; } = '.';

        public Room(string name,IWorld world)
        {
            Name = name;
            World = world;
        }

        public virtual IEnumerable<IAction> GetFinalActions()
        {
            return BaseActions;
        }
        public virtual StatsCollection GetFinalStats()
        {
            return BaseStats;
        }
        public virtual IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return BaseBehaviours;
        }

        public Point3 GetPoint()
        {
            return World.Map.GetPoint(this);
        }

        public override string ToString()
        {
            return Name ?? "Unnamed Room";
        }
    }
}
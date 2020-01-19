using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Places
{
    public class Room : HasStats, IPlace
    {
        /// <inheritdoc/>
        public IFaction ControllingFaction { get; set; }

        /// <inheritdoc/>
        public bool IsExplored { get; set; }

        public IWorld World { get; set; }

        /// <inheritdoc/>
        public char Tile { get; set; } = '.';

        public HashSet<IItem> Items { get;set; } = new HashSet<IItem>();
        
        public HashSet<Direction> LeaveDirections { get; set; } = new HashSet<Direction>(new []{Direction.North,Direction.South,Direction.East,Direction.West});

        public Room(string name, IWorld world, char tile)
        {
            Name = name;
            World = world;

            Tile = tile;
        }

        public override IActionCollection GetFinalActions(IActor forActor)
        {
            return new ActionCollection(BaseActions.Union(Adjectives.SelectMany(a=>a.GetFinalActions(forActor))));
        }
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var stats = BaseStats.Clone();

            foreach (var statsCollection in Adjectives.Select(a => a.GetFinalStats(forActor))) 
                stats.Add(statsCollection);

            return stats;
        }
        public override IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            return new BehaviourCollection(BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours(forActor))));
        }

        public IEnumerable<IActor> Actors => World.Population.Where(p => p.CurrentLocation == this);

        public Point3 GetPoint()
        {
            return World.Map.GetPoint(this);
        }

        public bool Has<T>() where T : IAdjective
        {
            return Adjectives.Any(a => a is T);
        }

        public bool Has<T>(Func<T, bool> condition) where T : IAdjective
        {
            return Adjectives.Any(a => a is T t && condition(t));
        }

        public Room AllowUpDown(bool allow)
        {
            if (allow)
            {
                LeaveDirections.Add(Direction.Up);
                LeaveDirections.Add(Direction.Down);
            }
            else
            {
                LeaveDirections.Remove(Direction.Up);
                LeaveDirections.Remove(Direction.Down);
            }

            return this;
        }
    }
}
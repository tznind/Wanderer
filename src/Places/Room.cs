using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Places
{
    public class Room : HasStats, IPlace
    {
        /// <inheritdoc/>
        public bool IsExplored { get; set; }

        public IWorld World { get; set; }

        /// <inheritdoc/>
        public char Tile { get; set; } = '.';

        public HashSet<IItem> Items { get;set; } = new HashSet<IItem>();

        public Room(string name,IWorld world)
        {
            Name = name;
            World = world;
        }

        public override IEnumerable<IAction> GetFinalActions(IActor forActor)
        {
            return BaseActions.Union(Adjectives.SelectMany(a=>a.GetFinalActions(forActor)));
        }
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var stats = BaseStats.Clone();

            foreach (var statsCollection in Adjectives.Select(a => a.GetFinalStats(forActor))) 
                stats.Add(statsCollection);

            return stats;
        }
        public override IEnumerable<IBehaviour> GetFinalBehaviours(IActor forActor)
        {
            return BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours(forActor)));
        }

        public IEnumerable<IActor> Actors => World.Population.Where(p => p.CurrentLocation == this);

        public Point3 GetPoint()
        {
            return World.Map.GetPoint(this);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Items;
using Wanderer.Relationships;

namespace Wanderer.Rooms
{
    public class Room : HasStats, IRoom
    {
        /// <inheritdoc/>
        public IFaction ControllingFaction { get; set; }

        /// <inheritdoc/>
        public bool IsExplored { get; set; }

        public IWorld World { get; set; }

        /// <inheritdoc/>
        public char Tile { get; set; } = '.';

        public List<IItem> Items { get;set; } = new List<IItem>();
        
        public HashSet<Direction> LeaveDirections { get; set; } = new HashSet<Direction>(new []{Direction.North,Direction.South,Direction.East,Direction.West});
        
        private ConsoleColor _explicitColor = DefaultColor;

        public override ConsoleColor Color
        {
            //use the faction color unless we have an explicit room color set
            get =>
                _explicitColor != DefaultColor ? _explicitColor :
                    ControllingFaction?.Color ?? _explicitColor;
            set => _explicitColor = value;
        }

        public override IEnumerable<IHasStats> GetAllHaves()
        {
            return 
                base.GetAllHaves()
                    .Union(Actors.Where( a=>!a.Dead))
                    .Union(Actors.SelectMany(a=>a.GetAllHaves()));
        }

        public Room(string name, IWorld world, char tile)
        {
            Name = name;
            World = world;

            Tile = tile;
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
    }
}
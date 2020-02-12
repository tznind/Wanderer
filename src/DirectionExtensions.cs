using System;

namespace Wanderer
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir)
        {
            return dir switch
            {
                Direction.None => Direction.None,
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }
    }
}
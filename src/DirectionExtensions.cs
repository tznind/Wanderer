using System;

namespace Wanderer
{
    /// <summary>
    /// static Extension methods for the <see cref="Direction"/> enum
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Returns the opposite direction to <paramref name="dir"/> e.g. for East it returns West.  Note opposite of None is None
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
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
using System;

namespace StarshipWanderer
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.None:
                    return Direction.None;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.East:
                    return Direction.West;
                case Direction.West:
                    return Direction.East;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}
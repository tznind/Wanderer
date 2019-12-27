using System;

namespace StarshipWanderer
{
    /// <summary>
    /// Represents a point in 3d space in which X maps to EAST/WEST, Y maps to NORTH/SOUTH and Z maps to UP/DOWN
    /// </summary>
    public class Point3
    {
        /// <summary>
        /// Distance east (positive) / west (negative)
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Distance north (positive) / south (negative)
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Distance up (positive) / down (negative)
        /// </summary>
        public int Z { get; }

        public Point3(int x,int y,int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        protected bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public Point3 Offset(Direction direction,int offset)
        {
            switch (direction)
            {
                case Direction.None:
                    throw new ArgumentException("Offset Direction cannot be None");
                case Direction.Up:
                    return new Point3(X,Y,Z+offset);
                case Direction.Down:
                    return new Point3(X,Y,Z-offset);
                case Direction.North:
                    return new Point3(X,Y+offset,Z);
                case Direction.South:
                    return new Point3(X,Y-offset,Z);
                case Direction.East:
                    return new Point3(X+offset,Y,Z);
                case Direction.West:
                    return new Point3(X-offset,Y,Z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public override string ToString()
        {
            return X + "," + Y + "," + Z;
        }
    }
}
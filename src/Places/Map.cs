using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Wanderer.Actors;

namespace Wanderer.Places
{
    /// <summary>
    /// Map in Z,X,Y order
    /// </summary>
    public class Map : Dictionary<Point3,IRoom>
    {
        /// <summary>
        /// Returns the location in 3d space of the room
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public Point3 GetPoint(IRoom place)
        {
            try
            {
                return this.First(k => k.Value == place).Key;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"Supplied Place '{place}' was not in the current Map",ex);
            }
        }


        /// <summary>
        /// Returns rooms adjacent to the provided location
        /// </summary>
        /// <param name="place"></param>
        /// <param name="pathable">true to return only places where you can directly move in that direction</param>
        /// <returns></returns>
        public Dictionary<Direction, IRoom> GetAdjacentPlaces(IRoom place, bool pathable)
        {

            var origin = GetPoint(place);

            var toReturn = new Dictionary<Direction,IRoom>();
            
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if(direction == Direction.None)
                    continue;

                var offset = origin.Offset(direction,1);

                //if you can move there or you don't care about walls!
                if(place.LeaveDirections.Contains(direction) || !pathable)
                    if(ContainsKey(offset))
                        toReturn.Add(direction,this[offset]);
            }

            return toReturn;
        }

    }
}

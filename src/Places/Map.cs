using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    /// <summary>
    /// Map in Z,X,Y order
    /// </summary>
    public class Map : Dictionary<Point3,IPlace>
    {
        /// <summary>
        /// Returns the location in 3d space of the room
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public Point3 GetPoint(IPlace place)
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

    }
}

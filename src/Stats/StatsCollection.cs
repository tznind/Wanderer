using System;
using System.Collections.Generic;

namespace StarshipWanderer.Stats
{
    public class StatsCollection: Dictionary<Stat,int>
    {
        public StatsCollection()
        {
            foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
                if(stat != Stat.None)
                    Add(stat, 0);
        }

        /// <summary>
        /// Creates a new copy with the same values
        /// </summary>
        /// <returns></returns>
        public StatsCollection Clone()
        {
            var clone = new StatsCollection();
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    clone[s] = this[s];

            return clone;
        }

        /// <summary>
        /// Adds the stats of the <paramref name="other"/> to this (changes permanently
        /// this class)
        /// </summary>
        /// <param name="other"></param>
        public StatsCollection Add(StatsCollection other)
        {
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] += other[s];

            return this;
        }
        
        /// <summary>
        /// Subtracts the stats of the <paramref name="other"/> to this (changes permanently
        /// this class)
        /// </summary>
        /// <param name="other"></param>
        public StatsCollection Subtract(StatsCollection other)
        {
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] -= other[s];

            return this;
        }
        /// <summary>
        /// Performs the <paramref name="modify"/> function on all stats in the collection.
        ///  (changes permanently this class)
        /// </summary>
        /// <returns></returns>
        public StatsCollection SetAll(Func<int,int> modify)
        {   
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] = modify(this[s]);

            return this;
        }

    }
}
using System;
using System.Collections.Generic;

namespace StarshipWanderer.Stats
{
    public class StatsCollection: Dictionary<Stat,int>
    {
        public StatsCollection()
        {
            foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
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
                clone[s] = this[s];

            return clone;
        }

        /// <summary>
        /// Adds the stats of the <paramref name="other"/> to this (changes permanently
        /// this class)
        /// </summary>
        /// <param name="other"></param>
        public void Add(StatsCollection other)
        {
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                this[s] += other[s];

        }
    }
}
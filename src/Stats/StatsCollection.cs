using System;
using System.Collections.Generic;

namespace StarshipWanderer.Stats
{
    public class StatsCollection: Dictionary<Stat,double>, IAreIdentical<StatsCollection>
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
        public StatsCollection SetAll(Func<double,double> modify)
        {   
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] = modify(this[s]);

            return this;
        }

        public bool AreIdentical(StatsCollection other)
        {
            if (other == null)
                return false;

            if (other.Count != Count)
                return false;

            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if (s != Stat.None)
                    if (Math.Abs(this[s] - other[s]) > 0.001)
                        return false;

            return true;

        }
    }
}
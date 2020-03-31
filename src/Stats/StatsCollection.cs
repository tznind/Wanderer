using System;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Stats
{
    public class StatsCollection: Dictionary<Stat,double>, IAreIdentical
    {
        /// <summary>
        /// Creates a new stat collection with all stats initialized to 0
        /// </summary>
        public StatsCollection():this(0)
        {
        }
        
        
        /// <summary>
        /// Creates a new stat collection with all stats initialized to <paramref name="startingValue"/>
        /// </summary>
        public StatsCollection(double startingValue)
        {
            foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
                if(stat != Stat.None)
                    Add(stat, startingValue);
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
        public StatsCollection Increase(StatsCollection other)
        {
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] += other[s];

            return this;
        }

        /// <summary>
        /// Increases the <paramref name="stat"/> by <paramref name="value"/>
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public void Increase(Stat stat, double value)
        {
            this[stat] += value;
        }

        /// <summary>
        /// Decreases the <paramref name="stat"/> by <paramref name="value"/>
        /// (this is the same as calling Increase with a negative value)
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public void Decrease(Stat stat, double value)
        {
            Increase(stat,-value);
        }
        /// <summary>
        /// Subtracts the stats of the <paramref name="other"/> to this (changes permanently
        /// this class)
        /// </summary>
        /// <param name="other"></param>
        public StatsCollection Decrease(StatsCollection other)
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
        /// <summary>
        /// Performs the <paramref name="modify"/> function on all stats in the collection.
        ///  (changes permanently this class)
        /// </summary>
        /// <returns></returns>
        public StatsCollection SetAll(Func<Stat,double,double> modify)
        {   
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                if(s != Stat.None)
                    this[s] = modify(s,this[s]);

            return this;
        }

        public bool AreIdentical(object other)
        {
            if (other == null)
                return false;

            if (other is StatsCollection otherSc)
            {
                if (otherSc.Count != Count)
                    return false;

                foreach (Stat s in Enum.GetValues(typeof(Stat)))
                    if (s != Stat.None)
                        if (Math.Abs(this[s] - otherSc[s]) > 0.001)
                            return false;

                return true;
            }

            return false;
        }

        public bool IsEmpty()
        {
            return this.All(v => Math.Abs(v.Value) < 0.0001);

        }

        /// <summary>
        /// Multiply each stat by the given <paramref name="ratios"/>.
        /// </summary>
        /// <param name="ratios"></param>   /// <param name="invertForNegatives">true to invert the ratio for negative stats e.g. ratio of 0.5 would make 10 become 5
        /// but -10 would become -20 (instead of -5).)</param>
        /// <returns></returns>
        public StatsCollection Multiply(StatsCollection ratios,bool invertForNegatives)
        {
            foreach (var s in ratios)
            {
                if(invertForNegatives && this[s.Key] < 0)
                    this[s.Key] *= 1/s.Value;
                else
                    this[s.Key] *= s.Value;
            }

            return this;
        }
    }
}
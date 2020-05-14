using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Stats
{
    public class StatsCollection: IAreIdentical, IDictionary<Stat, double>
    {
        private double _startingValue = 0;

        //for Json Serialization
        public Dictionary<Stat,double> BaseDictionary = new Dictionary<Stat, double>();

        public int Count => BaseDictionary.Count;

        public ICollection<Stat> Keys => BaseDictionary.Keys;

        public ICollection<double> Values => BaseDictionary.Values;

        public bool IsReadOnly => false;

        public double this[Stat index]
        {
            get
            {
                if(BaseDictionary.TryGetValue(index, out double val))
                    return val;
                
                BaseDictionary.Add(index,_startingValue);

                return _startingValue;
            }
            set {
                if(BaseDictionary.ContainsKey(index))
                    BaseDictionary[index] = value;
                else
                    BaseDictionary.Add(index,value);
            }
        }

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
            _startingValue = startingValue;
        }

        public void Add(Stat stat, double startingValue)
        {
            BaseDictionary.Add(stat,startingValue);
        }

        /// <summary>
        /// Creates a new copy with the same values
        /// </summary>
        /// <returns></returns>
        public StatsCollection Clone()
        {
            var clone = new StatsCollection(_startingValue);
            foreach (Stat s in Keys.ToArray())
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
            foreach (Stat s in Keys.ToArray().Union(other.Keys.ToArray()))
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
            foreach (Stat s in Keys.ToArray().Union(other.Keys.ToArray()))
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
            foreach (Stat s in Keys.ToArray())
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
            foreach (Stat s in Keys.ToArray())
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

                foreach (Stat s in Keys.ToArray().Union(otherSc.Keys.ToArray()))
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
            foreach (var s in Keys.ToArray().Union(ratios.Keys))
            {
                if(invertForNegatives && this[s] < 0)
                    this[s] *= 1/ratios[s];
                else
                    this[s] *= ratios[s];
            }

            return this;
        }

        public bool ContainsKey(Stat key)
        {
            return BaseDictionary.ContainsKey(key);
        }

        public bool Remove(Stat key)
        {
            return BaseDictionary.Remove(key);
        }

        public bool TryGetValue(Stat key, out double value)
        {
            return BaseDictionary.TryGetValue(key,out value);
        }

        public void Add(KeyValuePair<Stat, double> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            BaseDictionary.Clear();
        }

        public bool Contains(KeyValuePair<Stat, double> item)
        {
            return BaseDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<Stat, double>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<Stat, double> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<Stat, double>> GetEnumerator()
        {
            return BaseDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return BaseDictionary.GetEnumerator();
        }
    }
}
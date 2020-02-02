using System.Collections.Generic;

namespace StarshipWanderer.Actors
{
    public class SlotCollection : Dictionary<string,int>
    {
        /// <summary>
        /// Creates a new deep copy of the collection
        /// </summary>
        /// <returns></returns>
        public SlotCollection Clone()
        {
            var clone = new SlotCollection();

            foreach (var kvp in this) 
                clone.Add(kvp.Key, kvp.Value);

            return clone;
        }
    }
}
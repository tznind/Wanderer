using System;
using System.Collections.Generic;

namespace Wanderer
{
    /// <summary>
    /// Dictionary of strings to objects which dynamically creates keys as you ask for them e.g. mydict["fish"] > 10 creates a new key entry fish (if one does not already exist) and returns 0 (default for int).  This class simplifies variable assignment/creation in scripts.
    /// </summary>
    public class DynamicDictionary
    {
        
        /// <summary>
        /// The underlying currently stored dictionary values.  Primarily for JSON serialization
        /// </summary>
        public Dictionary<string,dynamic> BaseDictionary = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Returns or sets the key value if exists otherwise creates the key value and assigns it the default value of the data type being fetched/stored
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public dynamic this[string index]
        {
            get { return BaseDictionary.TryGetValue(index, out object val) ? val : 0; }
            set {
                if(BaseDictionary.ContainsKey(index))
                    BaseDictionary[index] = value;
                else
                    BaseDictionary.Add(index,value);
            }
        }
    }
}
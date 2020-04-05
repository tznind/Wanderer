using System;
using System.Collections.Generic;

namespace Wanderer
{
    public class DynamicDictionary
    {
        
        //for Json Serialization
        public Dictionary<string,dynamic> BaseDictionary = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);

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
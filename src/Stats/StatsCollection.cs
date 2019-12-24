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
    }
}
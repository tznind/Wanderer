using System;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Stats
{
    public class StatDefinitions
    {
        ///<summary>
        /// Stats that have no intrinsic meaning within the Wanderer engine but are used in your narrative content / injury systems etc
        ///</summary>
        public HashSet<Stat> All = new HashSet<Stat>();
        
        public StatDefinitions()
        {

            Reset();
        }

        /// <summary>
        /// Remove all added stats and restore only those required for the core engine to function e.g. <see cref="Stat.Fight"/>
        /// </summary>
        public void Reset()
        {
            All.Clear();

            All.Add(Stat.Fight);
            All.Add(Stat.Coerce);
            All.Add(Stat.Initiative);
            All.Add(Stat.Value);
            
        }
        
        public Stat Get(string name)
        {
            return 
                All.FirstOrDefault(s=>s.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase))
                ?? throw new ArgumentException($"Unknown stat '{name}'");
        }

        public Stat GetOrAdd(string name)
        {
            var existing = All.FirstOrDefault(s=>s.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase));

            if(existing == null)
            {
                existing = new Stat(name);
                All.Add(existing);
            }

            return existing;
        }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Wanderer.Stats
{
    [TypeConverter(typeof(StatTypeConverter))]
    public class Stat
    {

        public string Name { get; }

        public static Stat Fight = new Stat("Fight");
        public static Stat Coerce = new Stat("Coerce");

        public static Stat Savvy = new Stat("Savvy");
        public static Stat Suave = new Stat("Suave");
        public static Stat Leadership = new Stat("Leadership");
        public static Stat Initiative = new Stat("Initiative");
        public static Stat Value = new Stat("Value");

        ///<summary>
        /// Stats that have no intrinsic meaning within the Wanderer engine but are used in your narrative content / injury systems etc
        ///</summary>
        public static HashSet<Stat> CustomStats = new HashSet<Stat>();

        private Stat(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<Stat> GetAll()
        {
            yield return Fight;
            yield return Coerce;
            yield return Savvy;
            yield return Suave;
            yield return Leadership;
            yield return Initiative;
            yield return Value;

            foreach(var custom in CustomStats)
                yield return custom;
        }

        public static Stat Get(string name)
        {
            return 
                GetAll().FirstOrDefault(s=>s.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase))
                ?? throw new ArgumentException($"Unknown stat '{name}'");
        }

        public static Stat GetOrAdd(string name)
        {
            var existing = GetAll().FirstOrDefault(s=>s.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase));

            if(existing == null)
            {
                existing = new Stat(name);
                CustomStats.Add(existing);
            }

            return existing;
        }
    }
}
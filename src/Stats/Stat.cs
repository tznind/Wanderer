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
        public static Stat Initiative = new Stat("Initiative");
        public static Stat Value = new Stat("Value");

        public Stat(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }


        protected bool Equals(Stat other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Stat) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
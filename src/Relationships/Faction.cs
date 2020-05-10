using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Factories;
using YamlDotNet.Serialization;

namespace Wanderer.Relationships
{
    public class Faction : HasStats,IFaction
    {
        public FactionRole Role { get; set; }

        
        public string[] Forenames { get; set; }
        public string[] Surnames { get; set; }

        public SlotCollection DefaultSlots { get; set; } = new SlotCollection();

        public Faction()
        {

        }

        public Faction(string name, FactionRole role)
        {
            Name = name;
            Role = role;
        }


        public override string ToString()
        {
            return Name;
        }
        
        public string GenerateName(Random r)
        {
            return 
                (
                    (Forenames != null && Forenames.Any() ? Forenames[r.Next(Forenames.Length)] : "") 
                    + " " +
                    (Surnames != null && Surnames.Any() ? Surnames[r.Next(Surnames.Length)]: "")
                )
                .Trim();
        }
    }
}
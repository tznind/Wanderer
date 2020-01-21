using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Factories;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Relationships
{
    public class Faction : IFaction
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

        public FactionRole Role { get; set; }

        [YamlIgnore]
        public INameFactory NameFactory { get; set; }

        public IActorFactory ActorFactory { get; set; }


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
    }
}
using StarshipWanderer.Factories;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Relationships
{
    public class Faction : HasStats,IFaction
    {
        public FactionRole Role { get; set; }

        [YamlIgnore]
        public INameFactory NameFactory { get; set; }

        public IActorFactory ActorFactory { get; set; }

        public IRoomFactory RoomFactory { get; set; }

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
using Wanderer.Actors;
using Wanderer.Factories;
using YamlDotNet.Serialization;

namespace Wanderer.Relationships
{
    public class Faction : HasStats,IFaction
    {
        public FactionRole Role { get; set; }

        [YamlIgnore]
        public INameFactory NameFactory { get; set; }

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
    }
}
namespace StarshipWanderer.Relationships
{
    public class Faction : IFaction
    {
        public string Name { get; set; }

        public FactionRole Role { get; set; }

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
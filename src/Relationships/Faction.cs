namespace StarshipWanderer.Relationships
{
    public class Faction : IFaction
    {
        public string Name { get; set; }

        public Faction(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
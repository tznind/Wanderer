namespace StarshipWanderer.Relationships
{
    public interface IFaction
    {
        string Name { get; set; }

        FactionRole Role { get; set; }
    }
}
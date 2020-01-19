using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public interface IFaction
    {
        string Name { get; set; }

        FactionRole Role { get; set; }

        /// <summary>
        /// Creates names for people in this faction
        /// </summary>
        public INameFactory NameFactory { get; set; }
    }
}
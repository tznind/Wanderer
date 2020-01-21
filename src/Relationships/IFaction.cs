using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Factories;

namespace StarshipWanderer.Relationships
{
    public interface IFaction
    {
        Guid Identifier { get; set; }

        string Name { get; set; }

        FactionRole Role { get; set; }

        /// <summary>
        /// Creates names for people in this faction
        /// </summary>
        public INameFactory NameFactory { get; set; }
    }
}
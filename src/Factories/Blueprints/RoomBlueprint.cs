using System;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Factories.Blueprints
{
    public class RoomBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Null if the room thematically fits any faction, otherwise the <see cref="IFaction.Identifier"/>
        /// </summary>
        public Guid? Faction { get; set; }

        /// <summary>
        /// The digit to render in maps for this room
        /// </summary>
        public char Tile { get; set; }

    }
}
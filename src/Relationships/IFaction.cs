using System;
using Wanderer.Actors;
using Wanderer.Factories;

namespace Wanderer.Relationships
{
    public interface IFaction : IHasStats
    {

        FactionRole Role { get; set; }

        /// <summary>
        /// Creates names for people in this faction
        /// </summary>
        public INameFactory NameFactory { get; set; }

        /// <summary>
        /// Custom slots for the faction e.g. 1 tail 8 legs for the Spider Kingdom
        /// </summary>
        SlotCollection DefaultSlots { get; set; } 

    }
}
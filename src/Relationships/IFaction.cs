using System;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Relationships
{
    public interface IFaction : IHasStats
    {

        FactionRole Role { get; set; }
        
        /// <summary>
        /// Forenames which should be picked from (if any) when an <see cref="ActorBlueprint"/> has no name
        /// </summary>
        string[] Forenames { get; set; }
        
        /// <summary>
        /// Surnames which should be picked from (if any) when an <see cref="ActorBlueprint"/> has no name
        /// </summary>
        string[] Surnames { get; set; }

        /// <summary>
        /// Creates names for people in this faction
        /// </summary>
        string GenerateName(Random r);

        /// <summary>
        /// Custom slots for the faction e.g. 1 tail 8 legs for the Spider Kingdom
        /// </summary>
        SlotCollection DefaultSlots { get; set; } 

    }
}
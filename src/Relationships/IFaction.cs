using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Factories;

namespace StarshipWanderer.Relationships
{
    public interface IFaction : IHasStats
    {

        FactionRole Role { get; set; }

        /// <summary>
        /// Creates names for people in this faction
        /// </summary>
        public INameFactory NameFactory { get; set; }

        /// <summary>
        /// Create actors that fit thematically with this faction
        /// </summary>
        IActorFactory ActorFactory { get; set; }
    }
}
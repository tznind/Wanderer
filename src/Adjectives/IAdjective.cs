using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjective : IHasStats
    {
        IHasStats Owner { get; set; }

        /// <summary>
        /// True if the world should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        bool IsPrefix { get; set; }

        bool IsActive();
    }
}

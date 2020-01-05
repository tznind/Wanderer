using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjective : IHasStats
    {
        /// <summary>
        /// The object to which the adjective is attached
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// True if the world should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        bool IsPrefix { get; set; }





    }
}

using System.Collections.Generic;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public interface IAdjective : IHasStats, IAreIdentical<IAdjective>
    {
        /// <summary>
        /// The object to which the adjective is attached
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// True if the world should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        bool IsPrefix { get; set; }

        /// <summary>
        /// Multiplicative modifiers to the decorated object.  e.g. Value 2 would
        /// multiply the Value of any object the adjective is on by 2
        /// </summary>
        public StatsCollection StatsRatio { get; set; }
    }
}

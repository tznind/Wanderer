using System.Collections.Generic;
using Wanderer.Behaviours;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Adjectives
{
    /// <summary>
    /// A state or modifier that applies to an object e.g. a gun could have the adjective 'Rusty'.  An adjective can modify the stats of the thing it is applied to as well as grant new actions, behaviours etc
    /// </summary>
    public interface IAdjective : IHasStats
    {
        /// <summary>
        /// The object to which the adjective is attached
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// True if the <see cref="IHasStats.Name"/> should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        bool IsPrefix { get; set; }

        /// <summary>
        /// Multiplicative modifiers to the decorated object.  e.g. Value 2 would
        /// multiply the Value of any object the adjective is on by 2
        /// </summary>
        StatsCollection StatsRatio { get; set; }

        /// <summary>
        /// Applies the <see cref="StatsRatio"/> as a multiplication operation on <paramref name="stats"/>.  This allows you to apply negative modifiers (e.g. Fight *= 0.5) after all other operations (e.g. summing all the other adjectives and modifiers that affect an item).
        /// </summary>
        /// <param name="stats"></param>
        /// <returns>A clone of <paramref name="stats"/> multiplied by the <see cref="StatsRatio"/></returns>
        StatsCollection Modify(StatsCollection stats);

        
        /// <summary>
        /// Describes effects which reduce, eliminate or amplify the impact of this
        /// effect
        /// </summary>
        Resistances Resist { get; set; }
        
        /// <summary>
        /// Adds an <see cref="ExpiryBehaviour"/> to this <see cref="IAdjective"/>
        /// </summary>
        /// <param name="duration">How long it lasts, expiry happens at the end of the
        /// round. Set to 1 for an effect that only lasts for the current round</param>
        /// <returns></returns>
        IAdjective WithExpiry(int duration);

    }
}

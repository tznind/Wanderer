using Wanderer.Adjectives;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    public class AdjectiveBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// True if the <see cref="IHasStats.Name"/> should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        public bool IsPrefix { get; set; }

        /// <summary>
        /// Multiplicative modifiers to the decorated object.  e.g. Value 2 would
        /// multiply the Value of any object the adjective is on by 2
        /// </summary>
        public StatsCollection StatsRatio { get; set; }

        /// <summary>
        /// Describes effects which reduce, eliminate or amplify the impact of this
        /// effect
        /// </summary>
        public Resistances Resist { get; set; }
    }
}
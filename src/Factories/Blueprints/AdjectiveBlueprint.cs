using Wanderer.Adjectives;
using Wanderer.Stats;

namespace Wanderer.Factories.Blueprints
{
    public class AdjectiveBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Multiplicative modifiers to the decorated object.  e.g. Value 2 would
        /// multiply the Value of any object the adjective is on by 2
        /// </summary>
        public StatsCollection StatsRatio { get; set; }
    }
}
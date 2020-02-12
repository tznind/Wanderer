using Wanderer.Adjectives;
using Wanderer.Stats;

namespace Wanderer.Factories.Blueprints
{
    public class AdjectiveBlueprint
    {
        public string Type { get; set; }

        /// <summary>
        /// If you want to re-brand this adjective set this name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If you want to adjust the <see cref="IHasStats.BaseStats"/>
        /// beyond what normally comes with the base <see cref="IAdjective"/>
        /// out of the box
        /// </summary>
        public StatsCollection AdjustStats { get; set; }
    }
}
using System;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Factories.Blueprints
{
    public class ActorBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// MandatoryItems which the actor must be carrying when created
        /// </summary>
        public ItemBlueprint[] MandatoryItems { get; set;} = new ItemBlueprint[0];

        
        /// <summary>
        /// Items which the actor may be carrying when created
        /// </summary>
        public ItemBlueprint[] OptionalItems { get; set;} = new ItemBlueprint[0];
    }
}
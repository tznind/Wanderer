using System;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Factories.Blueprints
{
    public class ActorBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Items which the actor may be carrying when created
        /// </summary>
        public ItemBlueprint[] Items { get; set;} = new ItemBlueprint[0];
    }
}
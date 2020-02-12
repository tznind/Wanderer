using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    public class ActorBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Optional, overrides the default slots (1 head, 2 arms etc) that an actor
        /// would normally have
        /// </summary>
        public SlotCollection Slots { get; set; }

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
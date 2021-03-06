﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to build an <see cref="IActor"/>
    /// </summary>
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
        /// The word that describes how the actor fights when not
        /// equipped with a weapon e.g. fisticuffs
        /// </summary>
        public string FightVerb { get; set; }
        
        /// <summary>
        /// Items which the actor may be carrying when created
        /// </summary>
        public ItemBlueprint[] OptionalItems { get; set;} = new ItemBlueprint[0];


        /// <summary>
        /// Returns the named blueprint if it is this one or exists amongst the <see cref="OptionalItems"/>, <see cref="MandatoryItems"/> etc
        /// </summary>
        public override HasStatsBlueprint TryGetBlueprint(string name)
        {
            return base.TryGetBlueprint(name) ??
            MandatoryItems.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null) ??
            (HasStatsBlueprint)OptionalItems.Select(a=>a.TryGetBlueprint(name)).FirstOrDefault(b=>b != null);
        }
    }
}
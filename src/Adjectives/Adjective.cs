using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Adjectives
{
    /// <inheritdoc cref="IAdjective"/>
    public class Adjective : HasStats,IAdjective
    {
        /// <inheritdoc />
        public IHasStats Owner { get; set; }
        
        /// <inheritdoc />
        public bool IsPrefix { get; set; }
        
        /// <inheritdoc />
        public StatsCollection StatsRatio { get; set; } = new StatsCollection(1);

        /// <summary>
        /// Describes effects which reduce, eliminate or amplify the impact of this
        /// effect
        /// </summary>
        public Resistances Resist { get; set; }
        
        [JsonConstructor]
        protected Adjective()
        {
            
        }

        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        public Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }

        
        /// <inheritdoc />
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var effect = Resist?.Calculate(forActor) ?? 1;

            return base.GetFinalStats(forActor).Clone().SetAll(v=>v*effect);
        }

        
        /// <inheritdoc cref="IAreIdentical.AreIdentical"/>
        public bool AreIdentical(IAdjective other)
        {
            return this.AreIdentical((IHasStats)other);
        }

        
        /// <inheritdoc />
        public StatsCollection Modify(StatsCollection stats)
        {
            return stats.Clone().Multiply(StatsRatio,true);
        }
        
        /// <inheritdoc />
        public IAdjective WithExpiry(int duration)
        {
            //add the expiry behaviour to the original owner
            Owner.BaseBehaviours.Add(new ExpiryBehaviour(this, duration));
            return this;
        }
    }
}
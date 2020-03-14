using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public class Adjective : HasStats,IAdjective
    {
        public IHasStats Owner { get; set; }

        public bool IsPrefix { get; set; }

        public StatsCollection StatsRatio { get; set; } = new StatsCollection(1);

        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        public Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }
        
        public bool AreIdentical(IAdjective other)
        {
            return this.AreIdentical((IHasStats)other);
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            //the owners base stats are returned separately, we need to apply an offset to that where appropriate
            var clone = Owner.BaseStats.Clone();
            
            //what it currently is
            var offset = clone.Clone();

            //what we would like it to be
            clone.Multiply(StatsRatio,true);

            offset = clone.Subtract(offset);

            return offset;
        }

        /// <summary>
        /// Adds an <see cref="ExpiryBehaviour"/> to this <see cref="IAdjective"/>
        /// </summary>
        /// <param name="duration">How long it lasts, expiry happens at the end of the
        /// round. Set to 1 for an effect that only lasts for the current round</param>
        /// <returns></returns>
        public IAdjective WithExpiry(int duration)
        {
            BaseBehaviours.Add(new ExpiryBehaviour(this, duration));
            return this;
        }
    }
}
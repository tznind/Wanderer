using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Adjectives
{
    public class Adjective : HasStats,IAdjective
    {
        public IHasStats Owner { get; set; }

        public bool IsPrefix { get; set; }

        public StatsCollection StatsRatio { get; set; } = new StatsCollection(1);

        /// <summary>
        /// Describes effects which reduce, eliminate or amplify the impact of this
        /// effect
        /// </summary>
        public Resistances Resist { get; set; }

        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        public Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var effect = Resist?.Calculate(forActor) ?? 1;

            return base.GetFinalStats(forActor).Clone().SetAll(v=>v*effect);
        }

        public bool AreIdentical(IAdjective other)
        {
            return this.AreIdentical((IHasStats)other);
        }

        
        public StatsCollection Modify(StatsCollection stats)
        {
            return stats.Clone().Multiply(StatsRatio,true);
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
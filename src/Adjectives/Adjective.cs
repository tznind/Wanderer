using System.Collections.Generic;
using System.Linq;
using Wanderer.Behaviours;

namespace Wanderer.Adjectives
{
    public abstract class Adjective : HasStats,IAdjective
    {
        public IHasStats Owner { get; set; }

        public bool IsPrefix { get; set; } = true;
        
        /// <summary>
        /// Creates a new adjective with name based on Type name
        /// </summary>
        protected Adjective(IHasStats owner)
        {
            Owner = owner;
            Name = GetType().Name;
        }
        
        public abstract IEnumerable<string> GetDescription();


        public bool AreIdentical(IAdjective other)
        {
            return this.AreIdentical((IHasStats)other);
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
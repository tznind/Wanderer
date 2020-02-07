using System.Collections.Generic;

namespace StarshipWanderer.Adjectives
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

    }
}
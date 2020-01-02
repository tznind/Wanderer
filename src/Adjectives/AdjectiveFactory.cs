using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Adjectives
{
    public class AdjectiveFactory : IAdjectiveFactory
    {
        public IEnumerable<IAdjective> GetAvailableAdjectives(IActor actor)
        {
            yield return new Attractive(actor);
            yield return new Strong(actor);
            yield return new Tough(actor);
        }

        
        public IEnumerable<IAdjective> GetAvailableAdjectives(IItem item)
        {
            yield return new Light(item);
            yield return new Strong(item);
        }
    }
}
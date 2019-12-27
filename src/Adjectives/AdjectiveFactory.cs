using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Adjectives
{
    public class AdjectiveFactory : IAdjectiveFactory
    {
        public IEnumerable<IAdjective> GetAvailableAdjectives(IActor actor)
        {
            yield return new Attractive(actor);
            yield return new Injured(actor);
        }

        
    }
}
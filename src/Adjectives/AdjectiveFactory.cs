using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Adjectives
{
    public class AdjectiveFactory : IAdjectiveFactory
    {
        public IEnumerable<IAdjective> GetAvailableAdjectives(IPlace place)
        {
            yield return new Dark(place);
            yield return new Stale(place);
        }

        public IEnumerable<IAdjective> GetAvailableAdjectives(IActor actor)
        {
            yield return new Attractive(actor);
            yield return new Strong(actor);
            yield return new Tough(actor);
            yield return new Medic(actor);
        }

        
        public IEnumerable<IAdjective> GetAvailableAdjectives(IItem item)
        {
            yield return new Light(item);
            yield return new Strong(item);
        }
    }
}
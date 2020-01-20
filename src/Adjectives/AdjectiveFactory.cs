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
        public IEnumerable<IAdjective> GetAvailableAdjectives<T>(T o) where T : IHasStats
        {
            if (o is IPlace place)
            {
                yield return new Dark(place);
                yield return new Stale(place);
                yield return new Rusty(place);
            }

            if (o is IActor actor)
            {
                yield return new Attractive(actor);
                yield return new Strong(actor);
                yield return new Tough(actor);
                yield return new Medic(actor);
                yield return new Giant(actor);
            }

            if (o is IItem item)
            {
                yield return new Light(item);
                yield return new Strong(item);
                yield return new SingleUse(item);
                yield return new Medic(item);
                yield return new Tough(item);
                yield return new Rusty(item);
            }
        }
    }
}
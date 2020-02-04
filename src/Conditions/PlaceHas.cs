using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Conditions
{
    public class PlaceHas<T> : ICondition<SystemArgs> where T : Adjective
    {
        public bool IsMet(SystemArgs forTarget)
        {
            IPlace place = forTarget.AggressorIfAny?.CurrentLocation;

            if (place == null && forTarget.Recipient is IActor a)
                place = a.CurrentLocation;
            
            if (place == null)
                place = forTarget.Recipient as IPlace;
            
            if(place == null)
                throw new NotSupportedException("Could not determine place from SystemArgs");

            return place.Adjectives.Any(j=>j is T);
        }

        public string? SerializeAsConstructorCall()
        {
            return $"PlaceHas<{typeof(T).Name}>()";
        }
    }
}
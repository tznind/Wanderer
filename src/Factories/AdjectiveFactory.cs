using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public class AdjectiveFactory : IAdjectiveFactory
    {
        public Type[] KnownAdjectives { get; }

        public AdjectiveFactory()
        {
            KnownAdjectives = typeof(IAdjective).Assembly.GetTypes().Where(t =>
                typeof(IAdjective).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).ToArray();
        }

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
                yield return new Giant(item);
            }
        }

        public IAdjective Create(IHasStats s, AdjectiveBlueprint blueprint)
        {
            var type = KnownAdjectives.SingleOrDefault(t=>t.Name.Equals(blueprint.Type)) ?? throw new ArgumentException($"Could not find IAdjective of Type '{blueprint.Type}'");

            var adjective = Create(s, type);

            if (blueprint.Name != null)
                adjective.Name = blueprint.Name;

            if (blueprint.AdjustStats != null)
                adjective.BaseStats.Add(blueprint.AdjustStats);

            return adjective;
        }

        public IAdjective Create(IHasStats s, Type adjectiveType)
        {
            if(!typeof(IAdjective).IsAssignableFrom(adjectiveType))
                throw new ArgumentException($"Expected an IAdjective but was a {adjectiveType}");

            foreach (var constructor in adjectiveType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if(parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(s))
                    return (IAdjective) constructor.Invoke(new object[]{s});
            }

            throw new ArgumentException("Could not find a valid constructor for IAdjective Type " + adjectiveType);
        }
    }
}
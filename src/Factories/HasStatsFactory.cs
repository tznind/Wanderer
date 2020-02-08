using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;

namespace StarshipWanderer.Factories
{
    public abstract class HasStatsFactory<T> where T : IHasStats
    {
        public IAdjectiveFactory AdjectiveFactory { get; set; }
        public HashSet<Guid> UniquesSpawned = new HashSet<Guid>();

        protected HasStatsFactory(IAdjectiveFactory adjectiveFactory)
        {
            AdjectiveFactory = adjectiveFactory;
        }
        
        public void Add<T2>(T o) where T2 : IAdjective
        {
            var match = AdjectiveFactory.GetAvailableAdjectives(o).OfType<T2>().FirstOrDefault();

            if (match == null)
                throw new ArgumentException($"AdjectiveFactory did not know how to make an item {typeof(T)}.  Try adding it to AdjectiveFactory.GetAvailableAdjectives(IItem)");

            o.Adjectives.Add(match);
        }
        
        /// <summary>
        /// True if the blueprint should be included in randomization choices
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Spawnable(HasStatsBlueprint b)
        {
            if (!b.Unique)
                return true;

            return !UniquesSpawned.Contains(b.Identifier ?? Guid.Empty);
        }

        /// <summary>
        /// Copies the basic properties shared by all <see cref="HasStatsBlueprint"/> onto the target (<paramref name="onto"/>)
        /// </summary>
        /// <param name="onto"></param>
        /// <param name="blueprint"></param>
        /// <param name="world"></param>
        /// <param name="defaultDialogueVerb">What do you do to initiate dialogue with this T, e.g. talk, read, look around etc</param>
        protected virtual void AddBasicProperties(T onto, HasStatsBlueprint blueprint,IWorld world, string defaultDialogueVerb)
        {
            if (blueprint.Unique)
                UniquesSpawned.Add(blueprint.Identifier ?? Guid.Empty);

            onto.Color = blueprint.Color;
            onto.Unique = blueprint.Unique;

            if (blueprint.Identifier.HasValue)
                onto.Identifier = blueprint.Identifier;

            //Adjectives the user definitely wants included
            if (blueprint.MandatoryAdjectives.Any())
                foreach (var a in blueprint.MandatoryAdjectives)
                    onto.Adjectives.Add(AdjectiveFactory.Create(onto, a));
            
            //pick 1 random adjective if blueprint lists any to pick from
            if (blueprint.OptionalAdjectives.Any())
                onto.Adjectives.Add(
                    AdjectiveFactory.Create(onto, blueprint.OptionalAdjectives.GetRandom(world.R))
                );

            if (blueprint.Stats != null)
                onto.BaseStats.Add(blueprint.Stats.Clone());

            if (blueprint.Dialogue != null)
            {
                onto.Dialogue = blueprint.Dialogue;
                if (onto.Dialogue.Verb == null)
                    onto.Dialogue.Verb = defaultDialogueVerb;

                //if you couldn't 'talk' to it before you can now
                if(!onto.BaseActions.OfType<DialogueAction>().Any())
                    onto.BaseActions.Add(new DialogueAction());
            }
        }
    }
}
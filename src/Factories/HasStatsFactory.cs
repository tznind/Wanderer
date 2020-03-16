using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public abstract class HasStatsFactory<T1,T2> where T1:HasStatsBlueprint where T2 : IHasStats
    {
        public HashSet<Guid> UniquesSpawned = new HashSet<Guid>();
        
        public List<T1> Blueprints { get; set; } = new List<T1>();

        /// <summary>
        /// True if the blueprint should be included in randomization choices
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual bool Spawnable(HasStatsBlueprint b)
        {
            //don't return fixed location stuff as a random choice
            if (b is RoomBlueprint r && r.FixedLocation != null)
                return false;

            if (!b.Unique)
                return true;

            return !UniquesSpawned.Contains(b.Identifier ?? Guid.Empty);
        }

        /// <summary>
        /// Copies the basic properties shared by all <see cref="HasStatsBlueprint"/> onto the target (<paramref name="onto"/>)
        /// </summary>
        /// <param name="onto"></param>
        /// <param name="blueprint"></param>
        /// <param name="defaultDialogueVerb">What do you do to initiate dialogue with this T, e.g. talk, read, look around etc</param>
        protected virtual void AddBasicProperties(T2 onto, T1 blueprint, string defaultDialogueVerb)
        {
            if (blueprint.Unique)
                UniquesSpawned.Add(blueprint.Identifier ?? Guid.Empty);

            if (blueprint.Actions.Any())
                onto.BaseActions = blueprint.Actions.Clone();

            onto.Color = blueprint.Color;
            onto.Unique = blueprint.Unique;

            if (blueprint.Identifier.HasValue)
                onto.Identifier = blueprint.Identifier;
            
            if (blueprint.Stats != null)
                onto.BaseStats.Increase(blueprint.Stats.Clone());

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
        
        protected virtual T1 GetBlueprint(Guid guid)
        {
            return Blueprints.FirstOrDefault(b => b.Identifier == guid) ??
                   throw new GuidNotFoundException($"Could not find {typeof(T1).Name} with Guid {guid}",guid);
        }

        protected virtual T1 GetBlueprint(string name)
        {
            //if the user passed a string that was actually a Guid
            if (Guid.TryParse(name, out Guid g))
                return GetBlueprint(g);

            return Blueprints.FirstOrDefault(
                       b => string.Equals(b.Name, name, StringComparison.CurrentCultureIgnoreCase)) ??
                   throw new NamedObjectNotFoundException($"Could not find {typeof(T1).Name} Named {name}", name);
        }

    }
}
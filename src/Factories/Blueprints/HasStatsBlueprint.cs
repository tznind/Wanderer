using System;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Factories.Blueprints
{
    public abstract class HasStatsBlueprint
    {
        /// <summary>
        /// Uniquely identifies instances created from this blueprint
        /// </summary>
        public Guid? Identifier { get; set; }

        /// <summary>
        /// Explicit name for this e.g. Centipede otherwise leave null to generate
        /// a random name from the faction <see cref="NameFactory"/> (null Name works for npc only)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Things the object has to say, if multiple then one is picked at random.
        /// These guids map to <see cref="DialogueNode"/>
        /// </summary>
        public DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// List of <see cref="IAdjective"/> type names (which must be supported by
        /// <see cref="IAdjectiveFactory"/>) from which to pick at random when creating
        /// </summary>
        public AdjectiveBlueprint[] OptionalAdjectives { get;set; } = new AdjectiveBlueprint[0];

        /// <summary>
        /// By default a subset of <see cref="OptionalAdjectives"/> are written to the
        /// objects created by this blueprint (e.g. depending on difficulty, luck etc).
        /// Set those that MUST always be added
        /// </summary>
        public AdjectiveBlueprint[] MandatoryAdjectives { get; set; } = new AdjectiveBlueprint[0];

        /// <summary>
        /// The BaseStats to give the object
        /// </summary>
        public StatsCollection Stats { get; set; } = new StatsCollection();
    }
}
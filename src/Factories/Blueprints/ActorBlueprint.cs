using System;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Factories.Blueprints
{
    public class ActorBlueprint
    {
        /// <summary>
        /// Explicit name for this actor type e.g. Centipede otherwise leave null to generate
        /// a random name from the faction <see cref="NameFactory"/>
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Things the npc has to say, if multiple then one is picked at random.
        /// These guids map to <see cref="DialogueNode"/>
        /// </summary>
        public DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// List of <see cref="IAdjective"/> type names (which must be supported by
        /// <see cref="IAdjectiveFactory"/>) from which to pick at random when creating
        /// this Npc
        /// </summary>
        public AdjectiveBlueprint[] OptionalAdjectives { get;set; } = new AdjectiveBlueprint[0];

        /// <summary>
        /// By default a subset of <see cref="OptionalAdjectives"/> are written to the
        /// objects created by this blueprint (e.g. depending on difficulty, luck etc).
        /// Set those that MUST always be on the actor
        /// </summary>
        public AdjectiveBlueprint[] MandatoryAdjectives { get; set; } = new AdjectiveBlueprint[0];

        /// <summary>
        /// The BaseStats to give the actor
        /// </summary>
        public StatsCollection Stats { get; set; } = new StatsCollection();

        /// <summary>
        /// Items which the actor may be carrying when created
        /// </summary>
        public ItemBlueprint[] Items { get; set;} = new ItemBlueprint[0];
    }
}
using System;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Factories.Blueprints
{
    public class RoomBlueprint
    {
        /// <summary>
        /// Null if the room thematically fits any faction, otherwise the <see cref="IFaction.Identifier"/>
        /// </summary>
        public Guid? Faction { get; set; }

        /// <summary>
        /// Explicit name for this room
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The digit to render in maps for this room
        /// </summary>
        public char Tile { get; set; }

        /// <summary>
        /// Things the room has to 'say', if multiple then one is picked at random.
        /// These guids map to <see cref="DialogueNode"/>
        /// </summary>
        public DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// List of <see cref="IAdjective"/> type names (which must be supported by
        /// <see cref="IAdjectiveFactory"/>) from which to pick at random when creating
        /// this Room (e.g. <see cref="Dark"/>)
        /// </summary>
        public AdjectiveBlueprint[] Adjectives { get;set; } = new AdjectiveBlueprint[0];

        /// <summary>
        /// The BaseStats that apply to everyone in the room, does this room make it harder to Fight
        /// </summary>
        public StatsCollection Stats { get; set; } = new StatsCollection();
    }
}
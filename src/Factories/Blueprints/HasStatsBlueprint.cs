using System;
using StarshipWanderer.Actions;
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
        /// True to only ever generate one of this thing
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Black=0
        /// DarkBlue=1
        /// DarkGreen=2
        /// DarkCyan=3
        /// DarkRed=4
        /// DarkMagenta= 5
        /// DarkYellow=6
        /// Gray=7
        /// DarkGray=8
        /// Blue=9
        /// Green=10
        /// Cyan=11
        /// Red=12
        /// Magenta=13 
        /// Yellow=14
        /// White=15
        /// </summary>
        public ConsoleColor Color { get; set; } = ConsoleColor.White;

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

        /// <summary>
        /// Option, if specified this list becomes the actions of the object
        /// replacing any existing actions they might otherwise get)
        /// </summary>
        public ActionCollection Actions { get; set; } = new ActionCollection();
    }
}
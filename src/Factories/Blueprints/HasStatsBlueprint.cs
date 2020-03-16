using System;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Dialogues;
using Wanderer.Relationships;
using Wanderer.Stats;

namespace Wanderer.Factories.Blueprints
{
    public abstract class HasStatsBlueprint
    {
        /// <summary>
        /// Uniquely identifies instances created from this blueprint
        /// </summary>
        public Guid? Identifier { get; set; }
        
        /// <summary>
        /// Null if the object thematically fits any faction, otherwise the <see cref="IFaction.Identifier"/>
        /// </summary>
        public Guid? Faction { get; set; }

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
        /// List of <see cref="IAdjective"/> from which to pick at random when creating.
        /// These can be either Guids, Names or Type names
        /// </summary>
        public string[] OptionalAdjectives { get;set; } = new string[0];

        /// <summary>
        /// By default a subset of <see cref="OptionalAdjectives"/> are written to the
        /// objects created by this blueprint (e.g. depending on difficulty, luck etc).
        /// Set those that MUST always be added. These can be either Guids, Names or Type names
        /// </summary>
        public string [] MandatoryAdjectives { get; set; } = new string[0];

        /// <summary>
        /// The BaseStats to give the object
        /// </summary>
        public StatsCollection Stats { get; set; } = new StatsCollection();

        /// <summary>
        /// Option, if specified this list becomes the actions of the object
        /// replacing any existing actions they might otherwise get)
        /// </summary>
        public ActionCollection Actions { get; set; } = new ActionCollection();

        /// <summary>
        /// Returns true if the blueprint is appropriate for the supplied
        /// <paramref name="f"/>.  Generic objects are always suitable for any
        /// faction but faction specific items only suit when their specific <paramref name="f"/>
        /// is supplied
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool SuitsFaction(IFaction f)
        {
            return SuitsFaction(f?.Identifier);
        }

        /// <summary>
        /// Returns true if the blueprint is appropriate for the supplied
        /// <paramref name="factionIdentifier"/>.  Generic objects are always suitable for any
        /// faction but faction specific items only suit when their specific <paramref name="factionIdentifier"/>
        /// is supplied.
        ///
        /// <para>Passing null (no specific faction) always returns true</para>
        /// </summary>
        /// <param name="factionIdentifier"></param>
        /// <returns></returns>
        public bool SuitsFaction(Guid? factionIdentifier)
        {
            
            return Faction == null || Equals(factionIdentifier , Faction);
        }

        

        public override string ToString()
        {
            return Name ?? Identifier?.ToString() ?? "Unamed " + GetType().Name;
        }
    }
}
using System;
using System.Collections.Generic;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public class DialogueOptionBlueprint
    {
        /// <summary>
        /// Determines which <see cref="DialogueNode"/> should follow from this option if it is picked. Leave null to terminate dialogue
        /// </summary>
        public Guid? Destination { get; set; }

        /// <summary>
        /// How angry / happy this dialogue option makes the person you are talk to (as the perceive you). Positive for happy negative for angry.
        /// </summary>
        public int? Attitude { get; set; }

        /// <summary>
        /// The human readable text for the option
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Lua code effects which happen if this dialogue option is picked (spawn items etc)
        /// </summary>
        public List<EffectBlueprint> Effect = new List<EffectBlueprint>();

        /// <summary>
        /// Lua code that determines if the option should be presented to the user or not.  Should start with  "return ".  If multiple then they are combined with AND.  If none then option will always be presented
        /// <example>return AggressorIfAny:Has('Pistol')</example>
        /// </summary>
        public List<ConditionBlueprint> Condition { get; set; } = new List<ConditionBlueprint>();

        /// <summary>
        /// Set to true to allow user to select this option only once
        /// </summary>
        public bool SingleUse { get; set; } = false;


        /// <summary>
        /// <para>Makes dialogue option transition from talking to one room / actor to 
        /// another (who must be present).  This combines a condition of the guid being
        /// present and the state update if the option is picked.</para>
        ///
        /// <para>Setting a <see cref="Transition"/> overrides <see cref="Destination"/> as
        /// the option (if picked) will instead go to the <see cref="DialogueInitiation.Next"/> of
        /// the referenced object</para>
        /// </summary>
        public Guid? Transition { get; set; }
    }
}
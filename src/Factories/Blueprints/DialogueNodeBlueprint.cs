using System;
using System.Collections.Generic;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories.Blueprints
{
    public class DialogueNodeBlueprint
    {
        /// <summary>
        /// Unique identifier for this branch/leaf of the dialogue tree
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// <para>Collection of <see cref="TextBlock"/> which will be concatenated to create a single piece of text  that gets displayed for this node (includes support for conditional bits of text).</para>
        /// </summary>
        public List<TextBlock> Body { get; set; } = new List<TextBlock>();

        /// <summary>
        /// Conditions which should be true before this tree will be run.  Only applies to the Root  of a dialogue tree (i.e. initiating Dialogue).  To control conditional navigation set conditions on the <see cref="Options"/> instead
        /// </summary>
        public List<ConditionBlueprint> Condition { get; set; } = new List<ConditionBlueprint>();

        /// <summary>
        /// Responses that can be picked, can include conditional options (e.g. only available if a stat is above a threshold)
        /// </summary>
        public List<DialogueOptionBlueprint> Options = new List<DialogueOptionBlueprint>();

    }
    
}namespace Wanderer.Factories
{
}
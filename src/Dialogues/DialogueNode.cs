using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Dialogues
{
    public class DialogueNode
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
        public List<ICondition<SystemArgs>> Condition { get; set; } = new List<ICondition<SystemArgs>>();

        /// <summary>
        /// Responses that can be picked, can include conditional options (e.g. only available if a stat is above a threshold)
        /// </summary>
        public List<DialogueOption> Options = new List<DialogueOption>();

        public override string ToString()
        {
            return Identifier.ToString();
        }

        public DialogueOption[] GetOptionsToShow(IWorld world, SystemArgs args)
        {
            return Options.Where(o =>
             !o.Exhausted && o.AllConditionsMet(world,args)).ToArray();
        }

        internal bool AllConditionsMet(IWorld world, SystemArgs args)
        {
            return Condition.All(c=>c.IsMet(args.World,args));
        }
    }
}
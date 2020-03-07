using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Dialogues
{
    public class DialogueNode
    {
        public Guid Identifier { get; set; }

        public List<TextBlock> Body { get; set; } = new List<TextBlock>();

        public List<ICondition<SystemArgs>> Require { get; set; } = new List<ICondition<SystemArgs>>();

        public List<DialogueOption> Options = new List<DialogueOption>();

        public override string ToString()
        {
            return Identifier.ToString();
        }

        public DialogueOption[] GetOptionsToShow()
        {
            return Options.Where(o => !o.Exhausted).ToArray();
        }
    }
}
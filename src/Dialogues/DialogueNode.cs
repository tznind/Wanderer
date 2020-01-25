using System;
using System.Collections.Generic;
using StarshipWanderer.Dialogues.Conditions;

namespace StarshipWanderer.Dialogues
{
    public class DialogueNode
    {
        public Guid Identifier { get; set; }

        public String Body { get; set; }

        public DialogueConditionCollection Conditions { get; set; } = new DialogueConditionCollection();

        public List<DialogueOption> Options = new List<DialogueOption>();
    }
}
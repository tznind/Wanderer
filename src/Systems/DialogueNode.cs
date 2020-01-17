using System;
using System.Collections.Generic;

namespace StarshipWanderer.Systems
{
    public class DialogueNode
    {
        public Guid Identifier { get; set; }

        public String Body { get; set; }

        public List<DialogueOption> Options = new List<DialogueOption>();
    }
}
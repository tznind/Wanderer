using System;
using System.Collections.Generic;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues
{
    public class DialogueNode
    {
        public Guid Identifier { get; set; }

        public TextBlock[] Body { get; set; }

        public List<ICondition<SystemArgs>> Require { get; set; } = new List<ICondition<SystemArgs>>();

        public List<DialogueOption> Options = new List<DialogueOption>();
    }
}
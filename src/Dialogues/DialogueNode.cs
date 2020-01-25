using System;
using System.Collections.Generic;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues
{
    public class DialogueNode
    {
        public Guid Identifier { get; set; }

        public String Body { get; set; }

        public Banter Suits { get; set; } = Banter.None;

        public List<DialogueOption> Options = new List<DialogueOption>();
    }

    public enum Banter
    {
        None = 0,
        Friend,
        Foe,
        Neutral,
    }
}
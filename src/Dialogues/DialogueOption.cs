using System;
using System.Collections.Generic;
using StarshipWanderer.Compilation;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues
{
    public class DialogueOption
    {
        public Guid? Destination { get; set; }

        public int? Attitude { get; set; }

        public string Text { get; set; }

        public List<IEffect> Effect = new List<IEffect>();

        public override string ToString()
        {
            return Text;
        }
    }
}
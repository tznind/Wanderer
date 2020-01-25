using System;

namespace StarshipWanderer.Dialogues
{
    public class DialogueOption
    {
        public Guid? Destination { get; set; }

        public int? Attitude { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
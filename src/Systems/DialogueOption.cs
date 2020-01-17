using System;

namespace StarshipWanderer.Systems
{
    public class DialogueOption
    {
        public Guid? Destination { get; set; }

        public int? Attitude { get; set; }

        public string Text { get; set; }
    }
}
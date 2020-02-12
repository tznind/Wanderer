using System;
using System.Linq;

namespace Wanderer.Dialogues
{
    public class DialogueInitiation
    {
        /// <summary>
        /// The doing word that describes what starts the dialogue e.g. "talk" or "read"
        /// </summary>
        public string Verb { get; set; }

        /// <summary>
        /// The unique identifier of the next bit of text that should be presented
        /// </summary>
        public Guid? Next { get; set; }


        /// <summary>
        /// Dialogues to run if there's nothing better to say (i.e. <see cref="Next"/> is null)
        /// </summary>
        public Guid[] Banter { get; set; } = new Guid[0];

        /// <summary>
        /// Returns true if there is nothing to show for this thing
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Verb) || Next == null && !Banter.Any();
    }
}
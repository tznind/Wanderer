using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Dialogues
{
    public class DialogueOption
    {
        public Guid? Destination { get; set; }

        public int? Attitude { get; set; }

        public string Text { get; set; }

        public List<IEffect> Effect = new List<IEffect>();

        /// <summary>
        /// Set to true to allow user to select this option only once
        /// </summary>
        public bool SingleUse { get; set; } = false;

        /// <summary>
        /// Set to true to indicate that the option shouldn't be offered again
        /// (e.g. for <see cref="SingleUse"/> options)
        /// </summary>
        public bool Exhausted { get; set; } = false;


        public override string ToString()
        {
            return Text;
        }
    }
}
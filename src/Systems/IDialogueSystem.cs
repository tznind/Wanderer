using System;
using System.Collections.Generic;
using System.Text;
using Wanderer.Actors;
using Wanderer.Dialogues;

namespace Wanderer.Systems
{
    public interface IDialogueSystem : ISystem
    {
        List<DialogueNode> AllDialogues { get; set; }
        
        /// <summary>
        /// Run an explicit piece of dialogue out of the blue. <see cref="ISystem.Apply"/> instead for normal talking e.g. to an <see cref="IActor"/>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="node"></param>
        void Run(SystemArgs args, DialogueNode node);

        /// <summary>
        /// Lookup an explicit piece of dialogue
        /// </summary>
        /// <param name="g"></param>
        /// <returns>The dialogue found or null</returns>
        DialogueNode GetDialogue(Guid? g);
    }
}

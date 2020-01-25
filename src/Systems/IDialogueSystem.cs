using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Dialogues;

namespace StarshipWanderer.Systems
{
    public interface IDialogueSystem : ISystem
    {
        IList<DialogueNode> AllDialogues { get; set; }

        bool CanTalk(IActor actor, IActor other);

        /// <summary>
        /// When someone decides they want to talk who can they talk to?
        /// e.g. people in the same room as them? or do they have a radio
        /// to talk to someone elsewhere?
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        IEnumerable<IActor> GetAvailableTalkTargets(IActor actor);

        /// <summary>
        /// Run an explicit piece of dialogue out of the blue.  Use <see cref="CanTalk"/>
        /// and <see cref="ISystem.Apply"/> instead for normal talking e.g. to an <see cref="IActor"/>
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

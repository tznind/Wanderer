using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;

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
        /// Return a conversation suitable for the <paramref name="actor"/> or null
        /// </summary>
        /// <returns></returns>
        DialogueNode GetBanter(IActor actor);
    }
}

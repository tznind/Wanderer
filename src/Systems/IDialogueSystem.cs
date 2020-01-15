using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Systems
{
    public interface IDialogueSystem : ISystem
    {
        bool CanTalk(IActor actor, IActor other);
        IEnumerable<IActor> GetAvailableTalkTargets(IActor actor);
    }
}

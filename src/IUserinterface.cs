using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    public interface IUserinterface
    {
        void ShowActorStats(IActor actor);

        T GetOption<T>(string title) where T : Enum;
        void Refresh();
    }
}
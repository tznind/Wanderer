using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    public interface IUserinterface
    {
        void ShowActorStats(IActor actor);

        T GetOption<T>(string title,string body) where T : Enum;
        void Refresh();
        void ShowMessage(string title, string body);
    }
}
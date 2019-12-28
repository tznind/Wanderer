using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    public interface IUserinterface
    {

        EventLog Log {get;}

        void ShowActorStats(IActor actor);

        /// <summary>
        /// Gets user to pick from one of the available <paramref name="options"/>
        /// with the option to cancel / pick none.
        ///
        /// <para>default(T) should be considered cancel (so make sure if T is an enum
        /// your Enum has a None entry at 0)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="chosen"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        bool GetChoice<T>(string title, string body, out T chosen, params T[] options);

        void Refresh();
        void ShowMessage(string title, string body,bool log);
    }
}
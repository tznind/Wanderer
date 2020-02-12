using System;
using Wanderer.Actors;

namespace Wanderer
{
    public interface IUserinterface
    {
        /// <summary>
        /// Trashes the current <see cref="IWorld"/> and creates a new one
        /// </summary>
        void NewGame();

        EventLog Log {get;}

        void ShowStats(IHasStats of);

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

        /// <summary>
        /// Explicitly brings the message to the users attention (do not use for spammed messages, use <see cref="Log"/> instead
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        void ShowMessage(string title, string body);

        /// <summary>
        /// Explicitly brings the message to the users attention the logs it in the event log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="showThenLog"></param>
        void ShowMessage(string title, LogEntry showThenLog);
    }
}
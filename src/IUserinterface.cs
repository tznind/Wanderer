using System;
using Wanderer.Actors;

namespace Wanderer
{
    /// <summary>
    /// Interface for the application GUI that is handling obtaining user decisions (e.g. picking a dialogue option)
    /// </summary>
    public interface IUserinterface
    {
        /// <summary>
        /// Record of all events taking place in the world subdivided by round
        /// </summary>
        EventLog Log {get;}

        /// <summary>
        /// When implemented in a derived class, should present to the user the stats of the given object
        /// </summary>
        /// <param name="of"></param>
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
        
        /// <summary>
        /// Explicitly brings the message to the users attention (do not use for spammed messages, use <see cref="Log"/> instead
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        void ShowMessage(string title, string body);
    }
}
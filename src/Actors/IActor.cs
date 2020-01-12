using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    /// <summary>
    /// An entity in a single location (at once) capable of performing actions (this includes the human player)
    /// </summary>
    public interface IActor : IHasStats
    {
        
        /// <summary>
        /// True if actor is dead
        /// </summary>
        bool Dead { get; set; }

        /// <summary>
        /// Where the <see cref="Actor"/> currently is
        /// </summary>
        IPlace CurrentLocation { get; set; }

        /// <summary>
        /// Items that the actor owns
        /// </summary>
        HashSet<IItem> Items { get;set; }

        /// <summary>
        /// All factions which you belong to
        /// </summary>
        HashSet<IFaction> FactionMembership { get; set; }

        /// <summary>
        /// Asks the actor to pick a target for T.  This could be direction to move
        /// someone to attack etc. <paramref name="attitude"/> indicates how naughty
        /// the act is 0 neutral (won't hurt anyone), high numbers are friendly, negative
        /// numbers are hostile actions.
        /// 
        /// <para>Returning default(T) indicates no desire to make a decision</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="chosen">The target the actor picked</param>
        /// <param name="options"></param>
        /// <param name="attitude">0 for neutral actions, positive for actions that are helpful (to <paramref name="chosen"/>), negative for actions that are hostile to <paramref name="chosen"/> </param>
        /// <returns>True if the actor wants to go ahead</returns>
        bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, double attitude);

        /// <summary>
        /// Move the actor from it's <see cref="CurrentLocation"/> to a <paramref name="newLocation"/>
        /// </summary>
        /// <param name="newLocation"></param>
        void Move(IPlace newLocation);

        /// <summary>
        /// Ends the life of the <see cref="Actor"/>
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        void Kill(IUserinterface ui, Guid round);

        /// <summary>
        /// Returns all other people in the <see cref="CurrentLocation"/>
        /// </summary>
        /// <returns></returns>
        IActor[] GetCurrentLocationSiblings();

        
        /// <summary>
        /// Returns true if the <see cref="IActor"/> has the supplied adjective (or optionally
        /// an item).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeItems"></param>
        /// <returns></returns>
        bool Has<T>( bool includeItems) where T:IAdjective;


        /// <summary>
        /// Returns true if the <see cref="IActor"/> has the supplied adjective (or optionally
        /// an item) that matches the <paramref name="condition"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeItems"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool Has<T>(bool includeItems,Func<T,bool> condition) where T : IAdjective;

        StatsCollection GetFinalStats();

        IEnumerable<IAction> GetFinalActions();


        IEnumerable<IBehaviour> GetFinalBehaviours();
    }
}

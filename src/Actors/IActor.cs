using System.Collections.Generic;
using NStack;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    /// <summary>
    /// An entity in a single location (at once) capable of performing actions (this includes the human player)
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Human readable name for the <see cref="Actor"/>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Where the <see cref="Actor"/> currently is
        /// </summary>
        IPlace CurrentLocation { get; set; }

        /// <summary>
        /// The <see cref="IAction"/> that the <see cref="Actor"/> can undertake regardless of gear, location etc.
        /// </summary>
        HashSet<IAction> BaseActions { get; set; }


        /// <summary>
        /// Human readable words that describe the current state of the <see cref="IActor"/>
        /// </summary>
        HashSet<IAdjective> Adjectives { get; set; }

        /// <summary>
        /// Actors stats before applying any modifiers
        /// </summary>
        StatsCollection BaseStats { get; set; }

        /// <summary>
        /// Determines how the <see cref="Actor"/> responds to events (regardless of location or gear etc)
        /// </summary>
        HashSet<IBehaviour> BaseBehaviours { get; set; }

        /// <summary>
        /// Returns all behaviours the <see cref="Actor"/> is currently exhibiting (super set of <see cref="BaseBehaviours"/> and any from gear, <see cref="IAdjective"/> etc)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBehaviour> GetFinalBehaviours();
        
        /// <summary>
        /// Returns the <see cref="BaseStats"/> adjusted for current gear, <see cref="Adjectives"/> etc
        /// </summary>
        /// <returns></returns>
        StatsCollection GetFinalStats();

        /// <summary>
        /// Returns the <see cref="BaseActions"/> plus any allowed by the <see cref="CurrentLocation"/>, gear, <see cref="Adjectives"/> etc
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAction> GetFinalActions();

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
        bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude);

        /// <summary>
        /// Move the actor from it's <see cref="CurrentLocation"/> to a <paramref name="newLocation"/>
        /// </summary>
        /// <param name="newLocation"></param>
        void Move(IPlace newLocation);

        /// <summary>
        /// Ends the life of the <see cref="Actor"/>
        /// </summary>
        /// <param name="ui"></param>
        void Kill(IUserinterface ui);
    }
}

using System.Collections.Generic;
using NStack;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public interface IActor
    {
        string Name { get; set; }

        HashSet<IAction> BaseActions { get; set; }

        HashSet<IAdjective> Adjectives { get; set; }

        StatsCollection BaseStats { get; set; }

        IEnumerable<IBehaviour> GetFinalBehaviours();
        
        StatsCollection GetFinalStats();

        IEnumerable<IAction> GetFinalActions(IWorld world, IPlace actorsPlace);

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
        /// Move the actor from it's <paramref name="currentLocation"/> to a <paramref name="newLocation"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="currentLocation"></param>
        /// <param name="newLocation"></param>
        void Move(IWorld world, IPlace currentLocation, IPlace newLocation);
    }
}

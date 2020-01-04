using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public interface IWorld
    {
        Random R { get; set; }

        You Player { get; }

        IRoomFactory RoomFactory { get; set; }

        Map Map { get; }

        HashSet<IActor> Population { get; }
        
        /// <summary>
        /// Returns all the behaviours that should respond to events in the world
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBehaviour> GetAllBehaviours();

        /// <summary>
        /// Runs the supplied <paramref name="playerAction"/> and then all Npc actions
        /// (including event notifications player feedback etc).
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="playerAction"></param>
        void RunRound(IUserinterface ui, IAction playerAction);
    }
}
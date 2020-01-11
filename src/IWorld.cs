using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;

namespace StarshipWanderer
{
    public interface IWorld
    {
        Random R { get; set; }

        You Player { get; }

        IRoomFactory RoomFactory { get; set; }

        Map Map { get; }

        HashSet<IActor> Population { get; }
        
        IFactionCollection Factions { get; set; }

        /// <summary>
        /// All known relationships in the <see cref="Population"/>
        /// </summary>
        IRelationshipSystem Relationships { get; set; }

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


        /// <summary>
        /// Attempts to completely erase the item from existence
        /// </summary>
        void Erase(IItem item);
    }
}
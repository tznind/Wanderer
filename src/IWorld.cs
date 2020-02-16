using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Places;
using Wanderer.Plans;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Wanderer
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
        /// System for handling communication including who can talk to who
        /// </summary>
        IDialogueSystem Dialogue { get; set; }

        /// <summary>
        /// All available injury systems for applying damage to people
        /// </summary>
        IList<IInjurySystem> InjurySystems { get; set; }

        /// <summary>
        /// System(s) for coerce, ordering, trade etc
        /// </summary>
        IList<INegotiationSystem> NegotiationSystems { get; set; }
        
        /// <summary>
        /// System for helping <see cref="Npc"/> make sensible decisions
        /// </summary>
        PlanningSystem PlanningSystem { get; set; }

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

        /// <summary>
        /// Creates a new room suitable for adding at the given <paramref name="newPoint"/>
        /// </summary>
        /// <param name="newPoint"></param>
        /// <returns></returns>
        IPlace GetNewRoom(Point3 newPoint);
    }
}
using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Plans;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer
{
    public interface IWorld
    { 
        Random R { get; set; }

        You Player { get; }

        /// <summary>
        /// Returns the named factory blueprint (by Name or Identifier)
        /// </summary>
        HasStatsBlueprint GetBlueprint(string toSpawn);

        /// <summary>
        /// Creates <see cref="IRoom"/>
        /// </summary>
        IRoomFactory RoomFactory { get; set; }
        
        /// <summary>
        /// Creates <see cref="IActor"/>
        /// </summary>
        IActorFactory ActorFactory { get; set; }
        
        /// <summary>
        /// Factory for items which fit the actors theme of this factory
        /// </summary>
        IItemFactory ItemFactory { get; set; }

        /// <summary>
        /// Creates <see cref="IAdjective"/>
        /// </summary>
        IAdjectiveFactory AdjectiveFactory { get; set; }

        /// <summary>
        /// Creates <see cref="IAction"/>
        /// </summary>
        IActionFactory ActionFactory { get;  set; }

        /// <summary>
        /// Creates <see cref="IBehaviour"/>
        /// </summary>
         IBehaviourFactory BehaviourFactory { get; set; } 

        Map Map { get; }

        HashSet<IActor> Population { get; }
        
        IFList<IAction> Factions { get; set; }

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
        /// Lua code to execute before every script element, e.g. to declare global functions
        /// </summary>
        string MainLua { get; set; }

        /// <summary>
        /// All the stats supported by the game
        /// </summary>
        StatDefinitions AllStats { get; set; }

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
        IRoom GetNewRoom(Point3 newPoint);

        /// <summary>
        /// Marks the given <paramref name="location"/> as explored and spawns a room
        /// there if none exists yet.  Combine with <see cref="RoomBlueprint.FixedLocation"/>
        /// to show the player where a plot central room is
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        IRoom Reveal(Point3 location);

        ISystem GetSystem(Guid g);
        ISystem GetSystem(string name);

        /// <summary>
        /// Default injury system for allocating injuries during Fight actions (<see cref="IInjurySystem.IsDefault"/>).
        /// Can return null if no injury systems are defined.  If none are <see cref="IInjurySystem.IsDefault"/> then first
        /// is returned
        /// </summary>
        /// <returns></returns>
        IInjurySystem GetDefaultInjurySystem();
    }
}
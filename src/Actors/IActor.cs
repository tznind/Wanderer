using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actors
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
        /// The word that describes how the actor fights when not
        /// equipped with a weapon e.g. fisticuffs
        /// </summary>
        string FightVerb { get; set; }

        /// <summary>
        /// Where the <see cref="Actor"/> currently is
        /// </summary>
        IRoom CurrentLocation { get; set; }

        /// <summary>
        /// Items that the actor owns
        /// </summary>
        List<IItem> Items { get;set; }

        /// <summary>
        /// How many of each body part does the actor have in which he can equip stuff
        /// </summary>
        SlotCollection AvailableSlots { get; set; }
        
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
        void Move(IRoom newLocation);

        /// <summary>
        /// Ends the life of the <see cref="Actor"/>
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        /// <param name="reason"></param>
        void Kill(IUserinterface ui, Guid round, string reason);

        /// <summary>
        /// Returns all other people in the <see cref="CurrentLocation"/>
        /// </summary>
        /// <returns></returns>
        IActor[] GetCurrentLocationSiblings(bool includeDead);

        
        /// <summary>
        /// Returns true if the <see cref="IActor"/> has the supplied adjective (or optionally
        /// an item) that matches the <paramref name="name"/> which can be an <see cref="IHasStats.Identifier"/>.
        /// </summary>
        /// <returns></returns>
        bool Has(string name, bool includeItems);

        StatsCollection GetFinalStats();

        IEnumerable<IAction> GetFinalActions();


        IEnumerable<IBehaviour> GetFinalBehaviours();

        /// <summary>
        /// Returns true if the current actor is able to observe freely the
        /// actions of <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsAwareOf(IActor other);

        /// <summary>
        /// Return true if the item is one you can equip (have enough slots, are not
        /// already equipping!)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason">Reason that you cannot equip it</param>
        /// <returns></returns>
        bool CanEquip(IItem item, out string reason);
        
        /// <summary>
        /// Returns this actors perception of <paramref name="other"/> (bear in mind you
        /// might like them but they might hate you back)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        double AttitudeTo(IActor other);


        /// <summary>
        /// Returns the distance to the other <paramref name="actor"/> 
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        double DistanceTo(IActor actor);

        IActor BestFriend(bool inSameLocation, double threshold);
        IActor WorstEnemy(bool inSameLocation, double threshold);


        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>
        /// </summary>
        /// <param name="blue"></param>
        /// <returns></returns>
        IItem SpawnItem(ItemBlueprint blue);

        
        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>.
        /// </summary>
        /// <param name="g"></param>
        /// <exception cref="GuidNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(Guid g);
        
        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>
        /// </summary>
        /// <param name="name"></param>
        ///  <exception cref="NamedObjectNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(string name);

        /// <summary>
        /// Automatically equips the given <paramref name="item"/> if possible
        /// without performing any formal actions (e.g. <see cref="EquipmentAction"/>)
        /// </summary>
        /// <param name="item"></param>
        void Equip(IItem item);
    }
}

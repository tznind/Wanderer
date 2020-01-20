using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;

namespace StarshipWanderer.Items
{

    public interface IItem : IHasStats
    {
        /// <summary>
        /// If the item must be equipped to use then this indicates what slot it fills
        /// otherwise it will be null
        /// </summary>
        IItemSlot Slot { get; set; }

        /// <summary>
        /// True if the item has been equipped by someone
        /// </summary>
        bool IsEquipped { get; set; }

        bool IsErased { get; set; }

        /// <summary>
        /// Forces the owner to drops the item in the supplied <paramref name="owner"/> location
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="owner"></param>
        /// <param name="round"></param>
        void Drop(IUserinterface ui, IActor owner, Guid round);

        /// <summary>
        /// True if there is an active adjective
        /// </summary>
        /// <param name="owner"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Has<T>(IActor owner) where T : IAdjective;


        /// <summary>
        /// True if there is an active adjective matching the <paramref name="condition"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="condition"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Has<T>(IActor owner,Func<T,bool> condition) where T : IAdjective;

    }
}
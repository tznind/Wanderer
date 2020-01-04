using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer.Items
{
    public interface IItem : IHasStats
    {
        IActor OwnerIfAny { get; set; }

        /// <summary>
        /// Forces the owner to drops the item in the supplied <paramref name="dropLocation"/>
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="dropLocation">The room to drop into or null to use actors current location</param>
        /// <param name="round"></param>
        void Drop(IUserinterface ui, IPlace dropLocation, Guid round);
    }
}
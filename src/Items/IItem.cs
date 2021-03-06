﻿using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Items
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

        /// <summary>
        /// Set on items that should have been removed from the game to prevent
        /// later reprocessing e.g. when one behaviour deletes another behaviours
        /// owner before that behaviour has itself resolved
        /// </summary>
        bool IsErased { get; set; }
        
        /// <summary>
        /// Conditions the wielder must meet before being able to interact with this item
        /// </summary>
        List<ICondition> Require { get; set; }


        /// <summary>
        /// Conditions the wielder must meet before equiping the item
        /// this object
        /// </summary>
        List<ICondition> EquipRequire { get; set; }


        /// <summary>
        /// Conditions the wielder must meet before unequiping the item
        /// this object
        /// </summary>
         List<ICondition> UnEquipRequire { get; set; }

        /// <summary>
        /// Returns true if the item requirements are met.  This includes all
        /// <see cref="Require"/> but also that the item is equipped (if it
        /// has a required <see cref="Slot"/>)
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        bool RequirementsMet(IActor forActor);

        /// <summary>
        /// Forces the owner to drops the item in the supplied <paramref name="owner"/> location
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="owner"></param>
        /// <param name="round"></param>
        void Drop(IUserinterface ui, IActor owner, Guid round);

        /// <summary>
        /// Returns true if the <paramref name="actor"/> can use this item
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool CanUse(IActor actor, out string reason);


        /// <summary>
        /// Returns true if the <paramref name="actor"/> can equip this item
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool CanEquip(IActor actor, out string reason);

        /// <summary>
        /// Returns true if the <paramref name="actor"/> can unequip this item
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool CanUnEquip(IActor actor, out string reason);

        /// <summary>
        /// Returns the injury system of the item, or any <see cref="FightAction"/> it grants or null 
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        IInjurySystem GetBestInjurySystem(IActor forActor);
        
    }
}
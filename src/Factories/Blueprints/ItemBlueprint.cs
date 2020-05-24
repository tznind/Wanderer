using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Items;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to create instances of <see cref="IItem"/> and <see cref="IItemStack"/>
    /// </summary>
    public class ItemBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// Which slots are required to equip the item.
        /// </summary>
        public ItemSlot Slot { get; set; }

        /// <summary>
        /// A condition that must be met before the item can be used
        /// </summary>
        public List<ConditionBlueprint> Require { get; set; }


        /// <summary>
        /// A condition that must be met before the item can be equipped
        /// </summary>
        public List<ConditionBlueprint> EquipRequire { get; set; }


        /// <summary>
        /// A condition that must be met before the item can be taken off
        /// </summary>
        public List<ConditionBlueprint> UnEquipRequire { get; set; }

        /// <summary>
        /// Null for an item that cannot be stacked, 1+ for a <see cref="IItemStack"/> of the given size
        /// </summary>
        public int? Stack { get; set; }
    }
}
using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Items;

namespace StarshipWanderer.Factories
{
    public class ItemBlueprint
    {
        public string Name { get; set; }
        public DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// Which slots are required to equip the item.
        /// </summary>
        public ItemSlot Slot { get; set; }

        /// <summary>
        /// A condition that must be met before the item can be used
        /// </summary>
        public List<ICondition<IActor>> Require { get; set; }
    }
}
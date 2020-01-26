using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;

namespace StarshipWanderer.Factories
{
    public class ItemBlueprint
    {
        public string Name { get; set; }
        public DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// A condition that must be met before the item can be used
        /// </summary>
        public List<ICondition<IActor>> Require { get; set; }
    }
}
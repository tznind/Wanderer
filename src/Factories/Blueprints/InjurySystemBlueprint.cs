using System;
using System.Collections.Generic;
using System.Text;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    public class InjurySystemBlueprint
    {
        public Guid Identifier { get; set; }

        public Spreading Spreads { get; set; }

        /// <summary>
        /// What to call each level of injury, these are inclusive boundaries 
        /// </summary>
        public Dictionary<double, string> Descriptions = new Dictionary<double, string>();

        /// <summary>
        /// True to sync the description of injuries to the severity e.g. burning gets worse and becomes flaming
        /// but 'broken leg' doesn't become worse and turn into 'severed leg'
        /// </summary>
        public bool FluidDescriptions { get; set; }

    }
}

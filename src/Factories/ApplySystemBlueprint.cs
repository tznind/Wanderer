using System;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    /// <summary>
    /// Describes how to find and apply an <see cref="ISystem"/> of a given Intensity to a given target(s) e.g. inflicting a given injury system on everyone in a Room
    /// </summary>
    public class ApplySystemBlueprint
    {
        /// <summary>
        /// Indicates which system should be applied.async  You must supply either Identifier or Name
        /// </summary>
        public Guid? Identifier {get;set;}

        /// <summary>
        /// How strongly to apply the given system (if supported by the system).  Typically should be a value between 0 and 100 but varies by system.
        /// </summary>
        public double Intensity {get;set;}

        /// <summary>
        /// Indicates which system should be applied.async  You must supply either Identifier or Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Apply the system to everyone in the current room not just the <see cref="EffectBlueprint.Target"/> (e.g. to model splash damage)
        /// </summary>
        public bool All {get;set;}
    }
}
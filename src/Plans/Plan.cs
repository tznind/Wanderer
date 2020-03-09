using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Plans
{
    public class Plan
    {
        /// <summary>
        /// Human readable description of the plan
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique identifier for the plan for referencing in scripts
        /// </summary>
        public Guid? Identifier { get; set; }

        /// <summary>
        /// Conditions which all must be met for the Plan to be considered viable
        /// </summary>
        public List<ICondition<SystemArgs>> Condition { get; set; } = new List<ICondition<SystemArgs>>();

        /// <summary>
        /// Describes the action and targets that should be picked in order to carry out the Plan
        /// </summary>
        public IFrameSource Do { get; set; }

        /// <summary>
        /// How popular the plan is relative to other plans
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Returns the <see cref="Identifier"/> or <see cref="Name"/> of the Plan
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Identifier?.ToString() ?? Name ?? "Unamed Plan";
        }
    }
}

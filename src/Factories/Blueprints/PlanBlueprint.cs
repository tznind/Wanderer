using System;
using System.Collections.Generic;

namespace Wanderer.Factories.Blueprints
{
    public class PlanBlueprint
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
        public List<ConditionBlueprint> Condition { get; set; } = new List<ConditionBlueprint>();

        /// <summary>
        /// Describes the action and targets that should be picked in order to carry out the Plan
        /// </summary>
        public FrameSourceBlueprint Do { get; set; }

        /// <summary>
        /// How popular the plan is relative to other plans
        /// </summary>
        public double Weight { get; set; }
    }
}
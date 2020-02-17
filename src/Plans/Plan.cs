using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Plans
{
    public class Plan
    {
        public string Name { get; set; }

        public Guid? Identifier { get; set; }

        public List<ICondition<SystemArgs>> Condition { get; set; } = new List<ICondition<SystemArgs>>();

        public IFrameSource DoFrame { get; set; }

        public double Weight { get; set; }

        public override string ToString()
        {
            return Identifier?.ToString() ?? Name ?? "Unamed Plan";
        }
    }
}

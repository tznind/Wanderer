using System;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Condition : ICondition
    {
        public SystemArgsTarget Check { get; }
        public abstract bool IsMet(IWorld world, SystemArgs o);

        public Condition(SystemArgsTarget check)
        {
            Check = check;
        }
    }
}
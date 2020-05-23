using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Effect : IEffect
    {
        public SystemArgsTarget Target { get; }
        public abstract void Apply(SystemArgs args);

        public Effect(SystemArgsTarget check)
        {
            Target = check;
        }
    }
}
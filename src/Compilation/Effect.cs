using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Effect : IEffect
    {
        /// <inheritdoc cref="EffectBlueprint.Target"/>
        public SystemArgsTarget Target { get; }

        public abstract void Apply(SystemArgs args);

        public Effect(SystemArgsTarget target)
        {
            Target = target;
        }
    }
}
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Effect : IEffect
    {
        /// <inheritdoc cref="EffectBlueprint.Target"/>
        public SystemArgsTarget Target { get; }

        /// <inheritdoc />
        public abstract void Apply(SystemArgs args);

        protected Effect(SystemArgsTarget target)
        {
            Target = target;
        }
    }
}
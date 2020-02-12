using Wanderer.Systems;

namespace Wanderer.Compilation
{

    public interface IEffect
    {
        void Apply(SystemArgs args);
    }
}
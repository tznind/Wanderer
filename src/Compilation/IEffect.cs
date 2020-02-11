using StarshipWanderer.Systems;

namespace StarshipWanderer.Compilation
{

    public interface IEffect
    {
        void Apply(SystemArgs args);
    }
}
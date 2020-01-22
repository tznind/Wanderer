using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IRoomFactory
    {
        IPlace Create(IWorld world);
    }
}
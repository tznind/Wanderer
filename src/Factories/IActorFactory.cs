using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IActorFactory
    {
        void Create(IWorld world, IPlace place);
    }
}
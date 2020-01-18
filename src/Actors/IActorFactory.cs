using StarshipWanderer.Places;

namespace StarshipWanderer.Actors
{
    public interface IActorFactory
    {
        void Create(IWorld world, IPlace place);
    }
}
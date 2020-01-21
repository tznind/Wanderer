using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IRoomFactory
    {
        IActorFactory GenericActorFactory { get; set; }
        IPlace Create(IWorld world);
    }
}
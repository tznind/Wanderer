using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IRoomFactory
    {
        IActorFactory ActorFactory { get; set; }
        IPlace Create(IWorld world);
    }
}
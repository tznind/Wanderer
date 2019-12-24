using StarshipWanderer.Actions;

namespace StarshipWanderer.Places
{
    public interface IRoomFactory
    {
        IActorFactory ActorFactory { get; }
        IPlace Create(IWorld world);
    }
}
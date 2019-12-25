using StarshipWanderer.Actions;

namespace StarshipWanderer.Places
{
    public interface IRoomFactory
    {
        IActorFactory ActorFactory { get; set; }
        IPlace Create(IWorld world);
    }
}
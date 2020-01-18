using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public interface IRoomFactory
    {
        IActorFactory ActorFactory { get; set; }
        IPlace Create(IWorld world);
    }
}
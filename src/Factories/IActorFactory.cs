using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Factories
{
    public interface IActorFactory
    {
        IFaction FactionIfAny { get; set; }
        void Create(IWorld world, IPlace place);
    }
}
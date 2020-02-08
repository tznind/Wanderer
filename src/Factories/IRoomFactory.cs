using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IRoomFactory
    {
        IPlace Create(IWorld world);

        RoomBlueprint[] Blueprints { get; set; }

        bool Spawnable(HasStatsBlueprint b);
    }
}
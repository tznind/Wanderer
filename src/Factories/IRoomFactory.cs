using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public interface IRoomFactory
    {
        IPlace Create(IWorld world);
        IPlace Create(IWorld world, RoomBlueprint blueprint);

        RoomBlueprint[] Blueprints { get; set; }

        bool Spawnable(HasStatsBlueprint b);
    }
}
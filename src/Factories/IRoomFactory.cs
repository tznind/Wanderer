using Wanderer.Factories.Blueprints;
using Wanderer.Places;

namespace Wanderer.Factories
{
    public interface IRoomFactory
    {
        IPlace Create(IWorld world);
        IPlace Create(IWorld world, RoomBlueprint blueprint);

        RoomBlueprint[] Blueprints { get; set; }

        bool Spawnable(HasStatsBlueprint b);
    }
}
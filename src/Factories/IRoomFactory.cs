using Wanderer.Factories.Blueprints;
using Wanderer.Places;

namespace Wanderer.Factories
{
    public interface IRoomFactory
    {
        IRoom Create(IWorld world);
        IRoom Create(IWorld world, RoomBlueprint blueprint);

        RoomBlueprint[] Blueprints { get; set; }

        bool Spawnable(HasStatsBlueprint b);
    }
}
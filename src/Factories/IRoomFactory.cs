using System;
using System.Collections.Generic;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    public interface IRoomFactory: IHasStatsFactory
    {
        IRoom Create(IWorld world);
        IRoom Create(IWorld world, RoomBlueprint blueprint);
        IRoom Create(IWorld world, Point3 location);

        List<RoomBlueprint> Blueprints { get; set; }

        bool Spawnable(HasStatsBlueprint b);

        HasStatsBlueprint TryGetBlueprint(string name);
        HasStatsBlueprint TryGetBlueprint(Guid g);
    }
}
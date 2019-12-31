using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public class WorldFactory
    {
        public IWorld Create()
        {
            var world = new World();

            var roomFactory = new RoomFactory(new ActorFactory());
            var startingRoom = roomFactory.Create(world);

            world.Population.Add(new You("Wanderer",startingRoom));
            world.RoomFactory = roomFactory;
            
            world.Map.Add(new Point3(0,0,0),startingRoom);

            return world;
        }
    }
}
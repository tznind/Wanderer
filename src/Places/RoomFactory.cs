using StarshipWanderer.Actions;

namespace StarshipWanderer.Places
{
    public class RoomFactory:IRoomFactory
    {
        public IPlace Create(IWorld world)
        { 
            var gunBay = new Room(world)
            {
                Title = "Gun Bay " + world.R.Next(5000)
            };

            gunBay.AddAction(new LoadGunsAction(world));
            
            return gunBay;
        }
    }
}
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public class RoomFactory:IRoomFactory
    {
        public IActorFactory ActorFactory { get; set; }

        public RoomFactory(IActorFactory actorFactory)
        {
            ActorFactory = actorFactory;
        }

        public IPlace Create(IWorld world)
        { 
            var gunBay = new Room("Gun Bay " + world.R.Next(5000),world)
            {
                Tile = 'g'
            };
            
            //an action the player can perform in this room
            gunBay.AddAction(new LoadGunsAction(null));

            ActorFactory.Create(world, gunBay);

            return gunBay;
        }
    }
}
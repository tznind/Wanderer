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
            var gunBay = new Room()
            {
                Title = "Gun Bay " + world.R.Next(5000)
            };
            
            //the player can leave this room
            gunBay.AddAction(new Leave(world,world.Player));
            //an action the player can perform in this room
            gunBay.AddAction(new LoadGunsAction(world,world.Player));
            gunBay.AddAction(new FightAction(world,world.Player));

            foreach (IActor actor in ActorFactory.Create(gunBay))
                gunBay.AddActor(actor);

            return gunBay;
        }
    }
}
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;

namespace StarshipWanderer.Places
{
    public class RoomFactory:IRoomFactory
    {
        public IActorFactory ActorFactory { get; set; }
        public IItemFactory ItemFactory { get; }
        public AdjectiveFactory AdjectiveFactory { get; set; }

        public RoomFactory(IActorFactory actorFactory,IItemFactory itemFactory, AdjectiveFactory adjectiveFactory)
        {
            ActorFactory = actorFactory;
            ItemFactory = itemFactory;
            AdjectiveFactory = adjectiveFactory;
        }

        public IPlace Create(IWorld world)
        { 
            var gunBay = new Room("Gun Bay " + world.R.Next(5000),world)
            {
                Tile = 'g'
            };

            //give the room a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(gunBay).ToArray();
            gunBay.Adjectives.Add(availableAdjectives[world.R.Next(0, availableAdjectives.Length)]);
            
            //an action the player can perform in this room
            gunBay.BaseActions.Add(new LoadGunsAction());
            ActorFactory.Create(world, gunBay);

            return gunBay;
        }
    }
}
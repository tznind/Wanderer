using System.Collections.Generic;
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
            var rooms = GetAvailableRooms(world).ToArray();

            var room = rooms[world.R.Next(rooms.Length)];

            //give the room a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(room).ToArray();
            room.Adjectives.Add(availableAdjectives[world.R.Next(0, availableAdjectives.Length)]);
            
            //some friends in the room with you
            ActorFactory.Create(world, room);

            return room;
        }

        private IEnumerable<IPlace> GetAvailableRooms(IWorld world)
        {
            yield return new Room("Gun Bay " + world.R.Next(5000), world, 'g')
                .With(new LoadGunsAction());

            yield return new Room("Stair" + world.R.Next(5000), world, 's')
                .AllowUpDown(true);
        }
    }
}
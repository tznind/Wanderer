using System.Linq;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public class RoomFactory: HasStatsFactory<IPlace>, IRoomFactory
    {
        public RoomBlueprint[] Blueprints { get; set; } = new RoomBlueprint[0];

        public RoomFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
        }

        public IPlace Create(IWorld world)
        {
            return Create(world,Blueprints.GetRandom(world.R));
        }

        public IPlace Create(IWorld world, RoomBlueprint blueprint)
        {
            if (blueprint == null)
                return new Room("Empty Room",world,'e');
            
            //pick blueprint faction (or random one if it isn't themed to a specific faction)
            var faction = blueprint.Faction != null
                ? world.Factions.Single(f => f.Identifier == blueprint.Faction)
                : world.Factions.GetRandomFaction(world.R);

            var room = new Room(blueprint.Name, world, blueprint.Tile) {ControllingFaction = faction};

            AddBasicProperties(room,blueprint,world,"look");

            if (faction != null)
            {
                //create some random NPCs
                faction.ActorFactory?.Create(world, room, faction);

                var itemFactory = faction.ActorFactory?.ItemFactory;

                if (itemFactory != null && itemFactory.Blueprints.Any())
                {
                    var items = world.R.Next(3);
                    for (int i = 0; i < items; i++) 
                        room.Items.Add(itemFactory.Create(world, itemFactory.Blueprints.GetRandom(world.R)));
                }
            }
            
            return room;
        }
    }
}
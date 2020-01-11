using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer
{
    public class WorldFactory
    {
        public IWorld Create()
        {
            var world = new World();
            world.Factions = new FactionCollection();

            var mainFaction = new Faction("Crew");
            world.Factions.Add(mainFaction);
            world.Relationships.Add(new FactionRelationship(mainFaction){Attitude = 5});
            
            var adjectiveFactory = new AdjectiveFactory();
            var itemFactory = new ItemFactory(adjectiveFactory);

            var roomFactory = new RoomFactory(actorFactory: new ActorFactory(itemFactory, adjectiveFactory), itemFactory,adjectiveFactory);
            var startingRoom = roomFactory.Create(world);
            startingRoom.IsExplored = true;

            world.Population.Add(new You("Wanderer",startingRoom));
            world.RoomFactory = roomFactory;
            
            world.Map.Add(new Point3(0,0,0),startingRoom);

            return world;
        }
    }
}
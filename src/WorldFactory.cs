using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer
{
    public class WorldFactory : IWorldFactory
    {
        public virtual IWorld Create()
        {
            var world = new World();
            var lowFaction = new Faction("Sub-levels Crew", FactionRole.Civilian);
            var securityFaction = new Faction("Security", FactionRole.Establishment);
            var animalFaction = new Faction("Wildlife", FactionRole.Wildlife);
            var corruptedFaction = new Faction("Order of the Twisted Sigil", FactionRole.Opposition);

            world.Factions.Add(securityFaction);
            world.Relationships.Add(new IntraFactionRelationship(lowFaction,5));
            world.Relationships.Add(new IntraFactionRelationship(securityFaction,5));
            world.Relationships.Add(new IntraFactionRelationship(corruptedFaction, 5));
            
            world.Relationships.Add(new IntraFactionRelationship(animalFaction, 5));
            world.Relationships.Add(new ExtraFactionRelationship(animalFaction, -20));

            //general respect for the security (but they don't care back).
            world.Relationships.Add(new InterFactionRelationship(lowFaction,securityFaction,2));

            //They are opposed to everyone
            world.Relationships.Add(new ExtraFactionRelationship(corruptedFaction,-5));
            //But really hate these guys
            world.Relationships.Add(new InterFactionRelationship(corruptedFaction,securityFaction,-20));

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
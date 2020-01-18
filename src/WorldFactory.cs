using System.Collections.Generic;
using System.IO;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;

namespace StarshipWanderer
{
    public class WorldFactory : IWorldFactory
    {
        public const string DialogueDirectory = "./Resources/Dialogue/";
        public const string Forenames = "./Resources/Names/Forenames.txt";
        public const string Surnames = "./Resources/Names/Surnames.txt";

        public virtual IWorld Create()
        {
            var world = new World();

            GenerateFactions(world);
            
            world.Dialogue = new DialogueSystem(GetAllDialogueYaml().ToArray());

            var adjectiveFactory = GetAdjectiveFactory();
            var itemFactory = GetItemFactory(adjectiveFactory);

            var nameFactory = GetNameFactory();
            var actorFactory = GetActorFactory(itemFactory, adjectiveFactory,nameFactory);
            var roomFactory = GetRoomFactory(actorFactory,itemFactory,adjectiveFactory);

            var startingRoom = GetStartingRoom(roomFactory,world);
            startingRoom.IsExplored = true;
            world.Map.Add(new Point3(0,0,0),startingRoom);

            world.Population.Add(GetPlayer(startingRoom));
            world.RoomFactory = roomFactory;
            
            return world;
        }

        protected virtual INameFactory GetNameFactory()
        {
            return new NameFactory(File.ReadAllLines(Forenames),  File.ReadAllLines(Surnames));
        }

        protected virtual IAdjectiveFactory GetAdjectiveFactory()
        {
            return new AdjectiveFactory();
        }
        protected virtual IItemFactory GetItemFactory(IAdjectiveFactory adjectiveFactory)
        {
            return new ItemFactory(adjectiveFactory);
        }
        protected virtual IActorFactory GetActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory, INameFactory nameFactory)
        {
            return new ActorFactory(itemFactory, adjectiveFactory,nameFactory);
        }
        protected virtual IRoomFactory GetRoomFactory(IActorFactory actorFactory, IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory)
        {
            return new RoomFactory(actorFactory, itemFactory,adjectiveFactory);
        }
        protected virtual IPlace GetStartingRoom(IRoomFactory roomFactory, World world)
        {
            return roomFactory.Create(world);
        }
        protected virtual You GetPlayer(IPlace startingRoom)
        {
            return new You("Wanderer", startingRoom);
        }

        /// <summary>
        /// Generate <see cref="IFaction"/> and <see cref="IFactionRelationship"/> in the world
        /// </summary>
        /// <param name="world"></param>
        protected virtual void GenerateFactions(World world)
        {
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
        }

        public virtual IEnumerable<string> GetAllDialogueYaml()
        {
            return Directory.EnumerateFiles(DialogueDirectory,"*.yaml",SearchOption.AllDirectories).Select(f => File.ReadAllText(f));
        }
    }
}
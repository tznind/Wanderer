﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class WorldFactory : IWorldFactory
    {
        public const string DialogueDirectory = "Dialogue/";
        public const string FactionsDirectory = "Factions/";

        public string ResourcesDirectory { get; set; }

        public WorldFactory()
        {
            string entry = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            ResourcesDirectory = Path.Combine(entry == null ? Environment.CurrentDirectory : Path.GetDirectoryName(entry) ?? ".","Resources");
        }


        public virtual IWorld Create()
        {
            var world = new World();

            GenerateFactions(world);

            world.Dialogue = new DialogueSystem(GetAllDialogueYaml().ToArray());

            var adjectiveFactory = GetAdjectiveFactory();
            var itemFactory = GetItemFactory(adjectiveFactory);

            var actorFactory = GetGenericActorFactory(itemFactory, adjectiveFactory);
            var roomFactory = GetRoomFactory(actorFactory,itemFactory,adjectiveFactory);

            var startingRoom = GetStartingRoom(roomFactory,world);
            startingRoom.IsExplored = true;
            world.Map.Add(new Point3(0,0,0),startingRoom);

            world.Population.Add(GetPlayer(startingRoom));
            world.RoomFactory = roomFactory;
            
            return world;
        }

        protected virtual IAdjectiveFactory GetAdjectiveFactory()
        {
            return new AdjectiveFactory();
        }
        protected virtual IItemFactory GetItemFactory(IAdjectiveFactory adjectiveFactory)
        {
            return new ItemFactory(adjectiveFactory);
        }

        /// <summary>
        /// Return the actor factory when no faction specific one is available (e.g. when
        /// a room is discovered that is not controlled by any)
        /// </summary>
        /// <param name="itemFactory"></param>
        /// <param name="adjectiveFactory"></param>
        /// <returns></returns>
        protected virtual IActorFactory GetGenericActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory)
        {
            return new ActorFactory(itemFactory, adjectiveFactory);
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
            var dirs = Directory.GetDirectories(Path.Combine(ResourcesDirectory, FactionsDirectory));

            var deserializer = new Deserializer();

            foreach (var directory in dirs)
            {
                
                Faction f;
                var factionFile = Path.Combine(directory, "Faction.yaml");

                try
                {
                    f = deserializer.Deserialize<Faction>(File.ReadAllText(factionFile));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionFile}",e);
                }

                var factionActorsFile = Path.Combine(directory, "Actors.yaml");
                
                try
                {
                    var adjectiveFactory = new AdjectiveFactory();
                    var actorFactory = new YamlActorFactory(File.ReadAllText(factionActorsFile),f,new ItemFactory(adjectiveFactory),adjectiveFactory);

                    f.ActorFactory = actorFactory;
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionActorsFile}",e);
                }


                var forenames = new FileInfo(Path.Combine(directory,"Forenames.txt"));
                var surnames = new FileInfo(Path.Combine(directory, "Surnames.txt"));

                f.NameFactory = new NameFactory(forenames.Exists ? File.ReadAllLines(forenames.FullName) : new string[0],
                    surnames.Exists ? File.ReadAllLines(surnames.FullName) : new string[0]);

                world.Relationships.Add(new IntraFactionRelationship(f,5));

                world.Factions.Add(f);
            }

            foreach (IFaction f in world.Factions)
            {
                //wildlife hates everyone
                if(f.Role == FactionRole.Wildlife)
                    world.Relationships.Add(new ExtraFactionRelationship(f, -20));

                if (f.Role == FactionRole.Opposition)
                {
                    world.Relationships.Add(new ExtraFactionRelationship(f, -5));
                    foreach (IFaction establishment in world.Factions.Where(f=>f.Role == FactionRole.Establishment))
                        //hate cops especially
                        world.Relationships.Add(new InterFactionRelationship(f,establishment,-10));
                }

                if(f.Role == FactionRole.Civilian)
                    foreach (IFaction establishment in world.Factions.Where(f=>f.Role == FactionRole.Establishment))
                        //general respect for the security (but they don't care back).
                        world.Relationships.Add(new InterFactionRelationship(f,establishment,2));
            }
        }

        public virtual IEnumerable<string> GetAllDialogueYaml()
        {
            return Directory.EnumerateFiles(Path.Combine(ResourcesDirectory,DialogueDirectory),"*.yaml",SearchOption.AllDirectories).Select(File.ReadAllText);
        }
    }
}
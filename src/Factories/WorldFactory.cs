﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Compilation;
using StarshipWanderer.Factories.Blueprints;
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

        SlotCollection _defaultSlots;
        ItemBlueprint[] _defaultItems;

        public virtual IWorld Create()
        {
            var world = new World();

            _defaultSlots = GetDefaultSlots();
            _defaultItems = GetDefaultItems();

            GenerateFactions(world);

            world.Dialogue = new YamlDialogueSystem(GetAllDialogueYaml().ToArray());

            var adjectiveFactory = GetAdjectiveFactory();

            var roomFactory = GetRoomFactory(adjectiveFactory);

            var startingRoom = GetStartingRoom(roomFactory,world);
            startingRoom.IsExplored = true;
            world.Map.Add(new Point3(0,0,0),startingRoom);

            world.Population.Add(GetPlayer(startingRoom));
            world.RoomFactory = roomFactory;
            
            return world;
        }

        /// <summary>
        /// Return items suitable for any room/faction
        /// </summary>
        /// <returns></returns>
        protected virtual ItemBlueprint[] GetDefaultItems()
        {
            string defaultSlots = Path.Combine(ResourcesDirectory, "Items.yaml");

            if (File.Exists(defaultSlots))
            {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<ItemBlueprint[]>(File.ReadAllText(defaultSlots));
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultSlots,e);
                }
            }

            return new ItemBlueprint[0];
        }

        protected virtual SlotCollection GetDefaultSlots()
        {
            string defaultSlots = Path.Combine(ResourcesDirectory, "Slots.yaml");

            if (File.Exists(defaultSlots))
            {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<SlotCollection>(File.ReadAllText(defaultSlots));
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultSlots,e);
                }
            }

            return new SlotCollection();
        }

        protected virtual IAdjectiveFactory GetAdjectiveFactory()
        {
            return new AdjectiveFactory();
        }

        protected virtual IRoomFactory GetRoomFactory(IAdjectiveFactory adjectiveFactory)
        {
            var fi = new FileInfo(Path.Combine(ResourcesDirectory, "Rooms.yaml"));
            
            if(fi.Exists)
                return new YamlRoomFactory(File.ReadAllText(fi.FullName),adjectiveFactory);

            return new RoomFactory(adjectiveFactory);
        }
        protected virtual IPlace GetStartingRoom(IRoomFactory roomFactory, World world)
        {
            return roomFactory.Create(world);
        }
        protected virtual You GetPlayer(IPlace startingRoom)
        {
            return new You("Wanderer", startingRoom)
            {
                AvailableSlots = _defaultSlots.Clone()
            };
        }

        /// <summary>
        /// Generate <see cref="IFaction"/> and <see cref="IFactionRelationship"/> in the world
        /// </summary>
        /// <param name="world"></param>
        protected virtual void GenerateFactions(World world)
        {
            var dirs = Directory.GetDirectories(Path.Combine(ResourcesDirectory, FactionsDirectory));

            var adjectiveFactory = new AdjectiveFactory();

            foreach (var directory in dirs)
            {
                
                Faction f;
                var factionFile = Path.Combine(directory, "Faction.yaml");

                try
                {
                    f = Compiler.Instance.Deserializer.Deserialize<Faction>(File.ReadAllText(factionFile));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionFile}",e);
                }

                var factionActorsFile = Path.Combine(directory, "Actors.yaml");
                var factionItemsFile = Path.Combine(directory, "Items.yaml");
                var factionRoomsFile = Path.Combine(directory, "Rooms.yaml");

                var factionSlotsFile = Path.Combine(directory, "Slots.yaml");
                IItemFactory itemFactory;

                try
                {
                    if(File.Exists(factionItemsFile))
                        itemFactory = new YamlItemFactory(File.ReadAllText(factionItemsFile),adjectiveFactory);
                    else
                        itemFactory = new ItemFactory(adjectiveFactory);

                    //add default items that anyone could have
                    if (_defaultItems != null && _defaultItems.Any())
                        itemFactory.Blueprints = itemFactory.Blueprints.Union(_defaultItems).ToArray();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionItemsFile}",e);
                }
                
                try
                {
                    string slotsYaml = null;

                    if (File.Exists(factionSlotsFile))
                        slotsYaml = File.ReadAllText(factionSlotsFile);

                    var actorFactory = new YamlActorFactory(File.ReadAllText(factionActorsFile),slotsYaml,itemFactory,adjectiveFactory);

                    //if there are no faction specific slots use the defaults
                    if (string.IsNullOrWhiteSpace(slotsYaml) && actorFactory.DefaultSlots.Count == 0)
                        actorFactory.DefaultSlots = _defaultSlots.Clone();

                    f.ActorFactory = actorFactory;
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing faction actors/slots file in dir '{directory}'",e);
                }

                try
                {
                    var roomFactory = File.Exists(factionRoomsFile)
                        ? new YamlRoomFactory(File.ReadAllText(factionRoomsFile), adjectiveFactory)
                        : new RoomFactory(adjectiveFactory);

                    f.RoomFactory = roomFactory;
                    foreach (RoomBlueprint b in f.RoomFactory.Blueprints)
                        if (!b.Faction.HasValue)
                            b.Faction = f.Identifier;
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionRoomsFile}",e);
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
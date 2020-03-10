using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Plans;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    public class WorldFactory : IWorldFactory
    {
        public const string DialogueDirectory = "Dialogue";

        public const string RoomsDirectory = "Rooms";
        public const string FactionsDirectory = "Factions";

        public string ResourcesDirectory { get; set; }

        public WorldFactory()
        {
            ResourcesDirectory = Compiler.GetDefaultResourcesDirectory();
        }

        SlotCollection _defaultSlots;
        ItemBlueprint[] _defaultItems;

        public virtual IWorld Create()
        {
            var world = new World();
            world.ResourcesDirectory = ResourcesDirectory;

            world.InjurySystems = GetInjurySystems();

            _defaultSlots = GetDefaultSlots();
            _defaultItems = GetDefaultItems();

            world.PlanningSystem = GeneratePlans(world);

            GenerateFactions(world);

            world.Dialogue = GetDialogue();

            var adjectiveFactory = GetAdjectiveFactory();

            var roomFactory = GetRoomFactory(new DirectoryInfo(ResourcesDirectory),adjectiveFactory);

            var zero = new Point3(0, 0, 0);
            var startingRoom = roomFactory.Create(world,zero);
            startingRoom.IsExplored = true;
            world.Map.Add(zero,startingRoom);

            world.Population.Add(GetPlayer(startingRoom));
            world.RoomFactory = roomFactory;
            
            return world;
        }

        public virtual IList<IInjurySystem> GetInjurySystems()
        {
            var toReturn = new List<IInjurySystem>();

            var dir = Path.Combine(ResourcesDirectory, "InjurySystems");

            if (!Directory.Exists(dir))
                return toReturn;

            foreach (var file in Directory.GetFiles(dir,"*.yaml",SearchOption.AllDirectories))
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    
                    if(string.IsNullOrWhiteSpace(yaml))
                        continue;

                    toReturn.Add(Compiler.Instance.Deserializer.Deserialize<InjurySystem>(yaml));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error building InjurySystem from file '{file}'",e);
                }
            }

            return toReturn;
        }


        public PlanningSystem GeneratePlans(IWorld world)
        {
            string defaultPlans = Path.Combine(ResourcesDirectory, "Plans.yaml");
            var planning = new PlanningSystem();

            if (File.Exists(defaultPlans))
            {
                try
                {
                    planning.Plans.AddRange(Compiler.Instance.Deserializer.Deserialize<Plan[]>(File.ReadAllText(defaultPlans)));
                    
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultPlans,e);
                }
            }

            return planning;

        }

        /// <summary>
        /// Return items suitable for any room/faction
        /// </summary>
        /// <returns></returns>
        protected virtual ItemBlueprint[] GetDefaultItems()
        {
            string defaultItems = Path.Combine(ResourcesDirectory, "Items.yaml");

            if (File.Exists(defaultItems))
            {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<ItemBlueprint[]>(File.ReadAllText(defaultItems));
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultItems,e);
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

        protected virtual IRoomFactory GetRoomFactory(DirectoryInfo dir, IAdjectiveFactory adjectiveFactory)
        {
           
            //Load ./Rooms.yaml
            var fi = new FileInfo(Path.Combine(ResourcesDirectory, "Rooms.yaml"));

            var factory = new RoomFactory(adjectiveFactory);

            if(fi.Exists)
                factory.Blueprints.AddRange(GetRoomBlueprints(fi));


            //Load ./Rooms/MyCoolRoom.yaml ./Rooms/DecrepidRooms/MyCoolRoom.yaml etc            
            var sub = new DirectoryInfo(Path.Combine(ResourcesDirectory,RoomsDirectory));

            if(sub.Exists)
                factory.Blueprints.AddRange(GetRoomBlueprints(sub));

            return factory;
        }

        protected virtual IEnumerable<RoomBlueprint> GetRoomBlueprints(FileInfo fi)
        {
            try
            {
                return Compiler.Instance.Deserializer.Deserialize<RoomBlueprint[]>(File.ReadAllText(fi.FullName));
            }
            catch(Exception e)
            {
                throw new Exception($"Error loading Rooms in file {fi.FullName}",e);
            }
        }

        protected virtual IEnumerable<RoomBlueprint> GetRoomBlueprints(DirectoryInfo dir)
        {
            var found = new List<RoomBlueprint>();

            //everything in the current directory is assumed to be a room
            foreach(FileInfo f in dir.GetFiles("*.yaml"))
                found.AddRange(GetRoomBlueprints(f));

            foreach(DirectoryInfo sub in dir.GetDirectories())
            {
                //Don't load anything under ./Rooms/Dialogue/
                if(sub.Name.Equals(DialogueDirectory,StringComparison.CurrentCultureIgnoreCase))
                    continue;

                //But do load everything under./Rooms/DangerousRooms/
                found.AddRange(GetRoomBlueprints(sub));
            }

            return found;
        }

        protected virtual You GetPlayer(IRoom startingRoom)
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
            var factionsDir = Path.Combine(ResourcesDirectory, FactionsDirectory);

            if (!Directory.Exists(factionsDir))
                return;

            var dirs = Directory.GetDirectories(factionsDir);

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

                f.RoomFactory = GetRoomFactory(new DirectoryInfo(directory),adjectiveFactory);
                
                foreach (RoomBlueprint b in f.RoomFactory.Blueprints)
                    if (!b.Faction.HasValue)
                        b.Faction = f.Identifier;

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

        public virtual DialogueSystem GetDialogue()
        {
            var toReturn = new DialogueSystem();

            foreach (var dir in Directory.GetDirectories(ResourcesDirectory, DialogueDirectory, SearchOption.AllDirectories))
                toReturn.AllDialogues.AddRange(GetDialogue(new DirectoryInfo(dir)));
            
            return toReturn;
        }

        protected virtual IEnumerable<DialogueNode> GetDialogue(DirectoryInfo dialogueDir)
        {
            var toReturn = new List<DialogueNode>();
        
            foreach (var fi in dialogueDir.GetFiles("*.yaml",SearchOption.AllDirectories))
            {
                try
                {
                    foreach (var dialogueNode in Compiler.Instance.Deserializer.Deserialize<DialogueNode[]>(File.ReadAllText(fi.FullName))) 
                        toReturn.Add(dialogueNode);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Error in dialogue yaml:{ e.Message } in file '{fi.FullName}'",e);
                }
            }

            return toReturn;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //These are the types of directories/yaml files we expect to find
        
        public const string FactionsDirectory = "Factions";
        public const string DialogueDirectory = "Dialogue";
        public const string RoomsDirectory = "Rooms";
        public const string ItemsDirectory = "Items";
        public const string ActorsDirectory = "Actors";

        public string ResourcesDirectory { get; set; }

        public WorldFactory()
        {
            ResourcesDirectory = Compiler.GetDefaultResourcesDirectory();
        }

        SlotCollection _defaultSlots;
        List<ItemBlueprint> _defaultItems = new List<ItemBlueprint>();

        List<ActorBlueprint> _defaultActors = new List<ActorBlueprint>();
        
        /// <summary>
        /// Mapping between directories and the factions which were created from them
        /// </summary>
        private Dictionary<string,Faction> _factionDirs = new Dictionary<string, Faction>();

        public virtual IWorld Create()
        {
            var world = new World();
            world.ResourcesDirectory = ResourcesDirectory;

            world.InjurySystems = GetInjurySystems();

            _defaultSlots = GetDefaultSlots();

            world.PlanningSystem = GeneratePlans(world);

            GenerateFactions(world);

           var adjectiveFactory = GetAdjectiveFactory();

            world.Dialogue = new DialogueSystem();
            world.RoomFactory = new RoomFactory(adjectiveFactory);

            //Get every yaml file under the resources dir
            foreach(var fi in Directory.GetFiles(ResourcesDirectory,"*.yaml",SearchOption.AllDirectories).Select(f=>new FileInfo(f)))
            {
                //is a faction dir
                var dirs = fi.Directory.FullName.Split(Path.DirectorySeparatorChar);
                IFaction faction = null;

                var factionDir = _factionDirs.Keys.FirstOrDefault(k =>
                fi.Directory.FullName.StartsWith(k, StringComparison.CurrentCultureIgnoreCase));

                if (factionDir != null)
                    faction = _factionDirs[factionDir];

                if(IsRoomsFile(fi,dirs))
                    (faction?.RoomFactory ?? world.RoomFactory).Blueprints.AddRange(GetRoomBlueprints(fi));

                if(IsItemsFile(fi,dirs))
                    (faction?.ActorFactory?.ItemFactory?.Blueprints ?? _defaultItems).AddRange(GetItemBlueprints(fi));

                if(IsActorsFile(fi,dirs))
                    (faction?.ActorFactory?.Blueprints ?? _defaultActors).AddRange(GetActorBlueprints(fi));

                if(IsDialogueFile(fi,dirs))
                    world.Dialogue.AllDialogues.AddRange(GetDialogue(fi));
            }
            
            var zero = new Point3(0, 0, 0);
            var startingRoom = world.RoomFactory.Create(world,zero);
            startingRoom.IsExplored = true;
            world.Map.Add(zero,startingRoom);
            world.Population.Add(GetPlayer(startingRoom));
            
            return world;
        }

        private bool IsRoomsFile(FileInfo fi,string[] path)
        {
            return Is(fi,path,RoomsDirectory);
        }
        private bool IsActorsFile(FileInfo fi,string[] path)
        {
            return Is(fi,path,ActorsDirectory);
        }
        private bool IsItemsFile(FileInfo fi,string[] path)
        {
            return Is(fi,path,ItemsDirectory);
        }
        private bool IsDialogueFile(FileInfo fi,string[] path)
        {
            return Is(fi,path,DialogueDirectory);
        }
        private bool Is(FileInfo fi, string[] path, string typeOfFile)
        {

            if(fi.Name.Equals(typeOfFile + ".yaml",StringComparison.CurrentCultureIgnoreCase))
                return true;

            return string.Equals(GetLowestRecognizedDir(path) , typeOfFile,StringComparison.CurrentCultureIgnoreCase);
        }
        private string GetLowestRecognizedDir(string[] path)
        {
            
            return path.Reverse().FirstOrDefault(d=>
                d.Equals(RoomsDirectory,StringComparison.CurrentCultureIgnoreCase) ||
                d.Equals(ItemsDirectory,StringComparison.CurrentCultureIgnoreCase) ||
                d.Equals(ActorsDirectory,StringComparison.CurrentCultureIgnoreCase)||
                d.Equals(DialogueDirectory,StringComparison.CurrentCultureIgnoreCase)
            );
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
            var adjectiveFactory = GetAdjectiveFactory();

            if (!Directory.Exists(factionsDir))
                return;

            var dirs = Directory.GetDirectories(factionsDir);

            foreach (var directory in dirs)
            {
                Faction f;
                var factionFile = Path.Combine(directory, "Faction.yaml");
                var factionSlotsFile = Path.Combine(directory, "Slots.yaml");

                try
                {
                    f= Compiler.Instance.Deserializer.Deserialize<Faction>(File.ReadAllText(factionFile));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionFile}",e);
                }

                f.ActorFactory = new ActorFactory(new ItemFactory(adjectiveFactory),adjectiveFactory);
                f.RoomFactory = new RoomFactory(adjectiveFactory);

                try
                {
                    string slotsYaml = null;

                    if (File.Exists(factionSlotsFile))
                        slotsYaml = File.ReadAllText(factionSlotsFile);

                    //if there are no faction specific slots use the defaults
                    if (string.IsNullOrWhiteSpace(slotsYaml) && f.ActorFactory.DefaultSlots.Count == 0)
                        f.ActorFactory.DefaultSlots = _defaultSlots.Clone();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing faction actors/slots file in dir '{directory}'",e);
                }


                var forenames = new FileInfo(Path.Combine(directory,"Forenames.txt"));
                var surnames = new FileInfo(Path.Combine(directory, "Surnames.txt"));

                f.NameFactory = new NameFactory(forenames.Exists ? File.ReadAllLines(forenames.FullName) : new string[0],
                    surnames.Exists ? File.ReadAllLines(surnames.FullName) : new string[0]);

                //TODO: Really all factions just are mates with thier faction buddies and can't control that from yaml?
                world.Relationships.Add(new IntraFactionRelationship(f,5));

                _factionDirs.Add(directory,f);
                world.Factions.Add(f);
            }

            //TODO: Likewise this should be controllable elsewhere
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

        protected virtual IEnumerable<ItemBlueprint> GetItemBlueprints(FileInfo fi)
        {
            try
            {
                return Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(File.ReadAllText(fi.FullName));
            }
            catch(Exception e)
            {
                throw new Exception($"Error loading ItemBlueprints in file {fi.FullName}",e);
            }
        }

        protected virtual IEnumerable<ActorBlueprint> GetActorBlueprints(FileInfo fi)
        {
            try
            {
                return Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(File.ReadAllText(fi.FullName));
            }
            catch(Exception e)
            {
                throw new Exception($"Error loading ActorBlueprint in file {fi.FullName}",e);
            }
        }

        protected virtual IEnumerable<DialogueNode> GetDialogue(FileInfo fi)
        {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<DialogueNode[]>(File.ReadAllText(fi.FullName));
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Error in dialogue yaml:{ e.Message } in file '{fi.FullName}'",e);
                }
        }
    }
}
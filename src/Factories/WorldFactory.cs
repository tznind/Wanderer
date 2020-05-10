using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
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

        public const string DefaultActorBehavioursFile = "default.actor.behaviours.yaml";

        /// <summary>
        /// Path to yaml files read during <see cref="Create"/>
        /// </summary>
        public string ResourcesDirectory { get; set; }

        private readonly Logger _log;

        /// <summary>
        /// Create a new world using the default Resources directory path (which must exist)
        /// </summary>
        public WorldFactory()
        {
            ResourcesDirectory = Compiler.GetDefaultResourcesDirectory();
            _log = LogManager.GetCurrentClassLogger();
        }


        /// <summary>
        /// Create a new world using the Resources in the provided <paramref name="resourcesDirectory"/>.  If directory is empty then a basic world full of empty rooms will still be created
        /// </summary>
        /// <param name="resourcesDirectory"></param>
        public WorldFactory(string resourcesDirectory)
        {
            ResourcesDirectory = resourcesDirectory;
            _log = LogManager.GetCurrentClassLogger();

        }

        SlotCollection _defaultSlots;
        
        /// <summary>
        /// Mapping between directories and the factions which were created from them
        /// </summary>
        private readonly Dictionary<DirectoryInfo, Faction> _factionDirs = new Dictionary<DirectoryInfo, Faction>();

        /// <summary>
        /// True to skip loading items, actors, dialogue etc.  This leaves
        /// Only systems, adjectives etc being loaded
        /// </summary>
        public bool SkipContent {get;set;}

        public virtual IWorld Create()
        {
            var world = new World();
            world.ResourcesDirectory = ResourcesDirectory;

            _log.Info($"Resources Directory Is: {ResourcesDirectory}");

            world.InjurySystems = GetInjurySystems();

            _defaultSlots = GetDefaultSlots();

            world.PlanningSystem = GetPlanningSystem(world);

            GenerateFactions(world);

            world.AdjectiveFactory = GetAdjectiveFactory();
            world.Dialogue = new DialogueSystem();
            world.RoomFactory = new RoomFactory();
            world.ActorFactory = new ActorFactory()
            {
                DefaultSlots = _defaultSlots
            };

            world.ItemFactory = new ItemFactory();
            world.ActionFactory = new ActionFactory();
            world.BehaviourFactory = new BehaviourFactory();


            if(!Directory.Exists(ResourcesDirectory))
                throw new DirectoryNotFoundException($"Resources directory did not exist '{ResourcesDirectory}'");

            //Get every yaml file under the resources dir
            foreach(var fi in Directory.GetFiles(ResourcesDirectory,"*.yaml",SearchOption.AllDirectories).Select(f=>new FileInfo(f)))
            {
                var dir = new DirectoryInfo(ResourcesDirectory);

                _log.Info($"Loading ./{fi.FullName.Substring(dir.FullName.Length)}");

                //is a faction dir
                var dirs = fi.Directory.FullName.Split(Path.DirectorySeparatorChar);
                IFaction faction = null;

                var factionDir = _factionDirs.Keys.FirstOrDefault(k =>
                    
                fi.Directory.FullName.StartsWith(k.FullName, StringComparison.CurrentCultureIgnoreCase));

                if (factionDir != null)
                    faction = _factionDirs[factionDir];

                if(!SkipContent && IsRoomsFile(fi,dirs))
                    world.RoomFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<RoomBlueprint>(fi),faction));

                if(!SkipContent && IsItemsFile(fi,dirs))
                    world.ItemFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ItemBlueprint>(fi),faction));

                if(!SkipContent && IsActorsFile(fi,dirs))
                    world.ActorFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ActorBlueprint>(fi),faction));

                if(IsAdjectivesFile(fi,dirs))
                    world.AdjectiveFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<AdjectiveBlueprint>(fi),faction));

                if(IsActionsFile(fi,dirs))
                    world.ActionFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ActionBlueprint>(fi),faction));

                if (IsBehavioursFile(fi, dirs))
                {
                    var blueprints = AssignFaction(GetBlueprints<BehaviourBlueprint>(fi), faction).ToList();

                    if(fi.Name.Equals(DefaultActorBehavioursFile,StringComparison.CurrentCultureIgnoreCase))
                        world.ActorFactory.DefaultBehaviours.AddRange(blueprints);

                    world.BehaviourFactory.Blueprints.AddRange(blueprints);
                }
                    

                if(!SkipContent && IsDialogueFile(fi,dirs))
                    world.Dialogue.AllDialogues.AddRange(GetDialogue(fi));
            }
            
            LogBlueprints(world.RoomFactory.Blueprints);
            LogBlueprints(world.ActorFactory.Blueprints);
            LogBlueprints(world.ItemFactory.Blueprints);
            LogBlueprints(world.AdjectiveFactory.Blueprints);
            LogBlueprints(world.ActionFactory.Blueprints);
            LogBlueprints(world.BehaviourFactory.Blueprints);

            var zero = new Point3(0, 0, 0);
            var startingRoom = world.RoomFactory.Create(world,zero);
            startingRoom.IsExplored = true;
            world.Map.Add(zero,startingRoom);
            var player = GetPlayer(startingRoom);

            AddDefaults(world,player);
            
            return world;
        }

        public static void AddDefaults(IWorld world, IActor actor)
        {
            //add default actions (that all other actors would get)
            foreach (var a in world.ActorFactory.DefaultActions)
                if (a.Faction == null || actor.FactionMembership.Any(a.SuitsFaction))
                    world.ActionFactory.Create(world, actor, a);

            //add default behaviours (that all other actors would get)
            foreach (var b in world.ActorFactory.DefaultBehaviours)
                if (b.Faction == null || actor.FactionMembership.Any(b.SuitsFaction))
                    world.BehaviourFactory.Create(world, actor, b);
        }

        private void LogBlueprints<T>(List<T> blueprints) where T: HasStatsBlueprint
        {
            _log.Info("-----------------------------");
            _log.Info($"Found {blueprints.Count()} {typeof(T).Name}");
            _log.Info("-----------------------------");

            foreach(var blue in blueprints)
                _log.Info($"{blue} {(blue.Faction != null ? "(" + blue.Faction.ToString().Substring(0,8) +")" : "")}");
        }

        private IEnumerable<T> AssignFaction<T>(IEnumerable<T> blueprints, IFaction f) where T: HasStatsBlueprint
        {
            foreach (var blue in blueprints)
            {
                //if the blueprint doesn't have a specific faction listed, assign it one
                if (blue.Faction == null && f?.Identifier != null)
                    blue.Faction = f.Identifier;

                yield return blue;
            }
        }

        private bool IsRoomsFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("rooms.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsActorsFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("actors.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsAdjectivesFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("adjectives.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsActionsFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("actions.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsBehavioursFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("behaviours.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsItemsFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("items.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsDialogueFile(FileInfo fi,string[] path)
        {
            return fi.Name.EndsWith("dialogue.yaml",StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns all resources that matching the <paramref name="searchPattern"/>.  Default implementation is a recursive
        /// file search of the <see cref="ResourcesDirectory"/>.  Override this method to load resources from a remote API or
        /// zip file etc.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, string> GetFilesWithContent(string searchPattern)
        {
            return
                Directory.GetFiles(ResourcesDirectory, searchPattern, SearchOption.AllDirectories)
                    .ToDictionary(k => k, File.ReadAllText);
        }


        protected virtual Tuple<string, string> GetFileWithContent(string name)
        {
            var file = Path.Combine(ResourcesDirectory, name);
            return File.Exists(file) ? Tuple.Create(file, File.ReadAllText(file)) : null;
        }

        public virtual IList<IInjurySystem> GetInjurySystems()
        {
            var toReturn = new List<IInjurySystem>();

            foreach (var kvp in GetFilesWithContent("*injury.yaml"))
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(kvp.Value))
                        continue;

                    toReturn.Add(Compiler.Instance.Deserializer.Deserialize<InjurySystem>(kvp.Value));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error building InjurySystem from '{kvp.Key}'",e);
                }
            }
            
            return toReturn;
        }


        public virtual PlanningSystem GetPlanningSystem(IWorld world)
        {
            var defaultPlan = GetFileWithContent("plans.yaml");
            var planning = new PlanningSystem();

            if (defaultPlan != null)
            {
                try
                {
                    planning.Plans.AddRange(Compiler.Instance.Deserializer.Deserialize<Plan[]>(defaultPlan.Item2));
                    
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultPlan.Item1,e);
                }
            }

            return planning;

        }

        protected virtual SlotCollection GetDefaultSlots()
        {
            var defaultSlots = GetFileWithContent( "slots.yaml");

            if (defaultSlots != null)
            {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<SlotCollection>(defaultSlots.Item2);
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultSlots.Item1,e);
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
                AvailableSlots = _defaultSlots.Clone(),
                InjurySystem = startingRoom.World.GetDefaultInjurySystem()
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

            foreach (var directory in dirs)
            {
                Faction f;
                var factionFile = Path.Combine(directory, "faction.yaml");
                var factionSlotsFile = Path.Combine(directory, "slots.yaml");

                try
                {
                    f= Compiler.Instance.Deserializer.Deserialize<Faction>(File.ReadAllText(factionFile));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {factionFile}",e);
                }


                try
                {
                    string slotsYaml = null;

                    if (File.Exists(factionSlotsFile))
                        slotsYaml = File.ReadAllText(factionSlotsFile);

                    //if there are no faction specific slots use the defaults
                    if (string.IsNullOrWhiteSpace(slotsYaml))
                        f.DefaultSlots = _defaultSlots.Clone();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing faction actors/slots file in dir '{directory}'",e);
                }
                
                var forenames = new FileInfo(Path.Combine(directory,"forenames.txt"));
                var surnames = new FileInfo(Path.Combine(directory, "surnames.txt"));

                f.NameFactory = new NameFactory(forenames.Exists ? File.ReadAllLines(forenames.FullName) : new string[0],
                    surnames.Exists ? File.ReadAllLines(surnames.FullName) : new string[0]);

                //TODO: Really all factions just are mates with thier faction buddies and can't control that from yaml?
                world.Relationships.Add(new IntraFactionRelationship(f,5));

                _factionDirs.Add(new DirectoryInfo(directory),f);
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

        protected virtual IEnumerable<T> GetBlueprints<T>(FileInfo fi) where T:HasStatsBlueprint
        {
            try
            {
                return Compiler.Instance.Deserializer.Deserialize<List<T>>(File.ReadAllText(fi.FullName));
            }
            catch(Exception e)
            {
                throw new Exception($"Error loading {typeof(T).Name} in file {fi.FullName}",e);
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
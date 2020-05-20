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
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    /// <summary>
    /// Creates <see cref="IWorld"/> instances by deserializing yaml resource files.
    /// </summary>
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
        private readonly Dictionary<WorldFactoryResource, Faction> _factionDirs = new Dictionary<WorldFactoryResource, Faction>();

        /// <summary>
        /// True to skip loading items, actors, dialogue etc.  This leaves
        /// Only systems, adjectives etc being loaded
        /// </summary>
        public bool SkipContent {get;set;}

        public virtual IWorld Create()
        {
            var world = new World();

            _log.Info($"Resources Directory Is: {ResourcesDirectory}");

            LoadCustomStats(world);

            world.InjurySystems = GetInjurySystems();
            world.MainLua = GetResource("Main.lua")?.Content;

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

            //Get every yaml file under the resources dir
            foreach(var fi in GetResources("*.yaml"))
            {
                _log.Info($"Loading {fi.Location}");

                //is a faction dir
                IFaction faction = GetImplicitFactionFor(fi);
                
                if(!SkipContent && IsRoomsFile(fi))
                    world.RoomFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<RoomBlueprint>(fi),faction));

                if(!SkipContent && IsItemsFile(fi))
                    world.ItemFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ItemBlueprint>(fi),faction));

                if(!SkipContent && IsActorsFile(fi))
                    world.ActorFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ActorBlueprint>(fi),faction));

                if(IsAdjectivesFile(fi))
                    world.AdjectiveFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<AdjectiveBlueprint>(fi),faction));

                if(IsActionsFile(fi))
                    world.ActionFactory.Blueprints.AddRange(AssignFaction(GetBlueprints<ActionBlueprint>(fi),faction));

                if (IsBehavioursFile(fi))
                {
                    var blueprints = AssignFaction(GetBlueprints<BehaviourBlueprint>(fi), faction).ToList();

                    if(fi.Location.EndsWith(DefaultActorBehavioursFile,StringComparison.CurrentCultureIgnoreCase))
                        world.ActorFactory.DefaultBehaviours.AddRange(blueprints);

                    world.BehaviourFactory.Blueprints.AddRange(blueprints);
                }
                    

                if(!SkipContent && IsDialogueFile(fi))
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

        /// <summary>
        /// Load all custom named <see cref="Stat"/> for the game into <see cref="IWorld.AllStats"/>
        /// </summary>
        /// <param name="world"></param>
        protected virtual void LoadCustomStats(IWorld world)
        {
            var customStats = GetResource("stats.yaml");
            if (customStats != null)
            {
                try
                {
                    foreach (var stat in Compiler.Instance.Deserializer.Deserialize<List<Stat>>(customStats.Content)) 
                        world.AllStats.All.Add(stat);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to load custom stats from file {customStats.Location}",e);
                }
            }
        }

        /// <summary>
        /// Returns whether the given resource is declared with a hierarchy that exists below
        /// a given <see cref="IFaction"/> therefore the resource should be considered part of that
        /// faction.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected virtual IFaction GetImplicitFactionFor(WorldFactoryResource resource)
        {
            var factionDir = _factionDirs.Keys.FirstOrDefault(k =>
                k.SharesPath(resource));

            if (factionDir != null)
                return  _factionDirs[factionDir];

            return null;
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

        private bool IsRoomsFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("rooms.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsActorsFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("actors.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsAdjectivesFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("adjectives.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsActionsFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("actions.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsBehavioursFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("behaviours.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsItemsFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("items.yaml",StringComparison.CurrentCultureIgnoreCase);
        }
        private bool IsDialogueFile(WorldFactoryResource fi)
        {
            return fi.Location.EndsWith("dialogue.yaml",StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns all resources that matching the <paramref name="searchPattern"/>.  Default implementation is a recursive
        /// file search of the <see cref="ResourcesDirectory"/>.  Override this method to load resources from a remote API or
        /// zip file etc.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        protected virtual IEnumerable<WorldFactoryResource> GetResources(string searchPattern)
        {
            return
                Directory.GetFiles(ResourcesDirectory, searchPattern, SearchOption.AllDirectories)
                    .Select(k => new WorldFactoryResource(new FileInfo(k).FullName,File.ReadAllText(k)));
        }


        protected virtual WorldFactoryResource GetResource(string name)
        {
            var file = Path.Combine(ResourcesDirectory, name);
            return File.Exists(file) ? new WorldFactoryResource(new FileInfo(file).FullName,File.ReadAllText(file)) : null;
        }

        public virtual IList<IInjurySystem> GetInjurySystems()
        {
            var toReturn = new List<IInjurySystem>();

            foreach (var r in GetResources("*injury.yaml"))
            {
                if(r != null)
                    try
                    {
                        if(string.IsNullOrWhiteSpace(r.Content))
                            continue;

                        toReturn.Add(Compiler.Instance.Deserializer.Deserialize<InjurySystem>(r.Content));
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error building InjurySystem from '{r.Location}'",e);
                    }
            }
            
            return toReturn;
        }


        public virtual PlanningSystem GetPlanningSystem(IWorld world)
        {
            var defaultPlan = GetResource("plans.yaml");
            var planning = new PlanningSystem();

            if (defaultPlan != null)
            {
                try
                {
                    var blueprints = Compiler.Instance.Deserializer.Deserialize<PlanBlueprint[]>(defaultPlan.Content);
                    var planFactory = new PlanFactory();

                    foreach(var blue in blueprints)
                        planning.Plans.Add(planFactory.Create(blue));
                    
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultPlan.Location,e);
                }
            }

            return planning;

        }

        protected virtual SlotCollection GetDefaultSlots()
        {
            var defaultSlots = GetResource( "slots.yaml");

            if (defaultSlots != null)
            {
                try
                {
                    return Compiler.Instance.Deserializer.Deserialize<SlotCollection>(defaultSlots.Content);
                }
                catch (Exception e)
                {
                    throw new Exception("Error deserializing " + defaultSlots.Location,e);
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
            foreach (var r in GetResources("*faction.yaml"))
            {
                Faction f;
                try
                {
                    f= Compiler.Instance.Deserializer.Deserialize<Faction>(r.Content);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error Deserializing file {r.Location}",e);
                }

                //TODO: Really all factions just are mates with thier faction buddies and can't control that from yaml?
                world.Relationships.Add(new IntraFactionRelationship(f,5));
                
                if(!_factionDirs.ContainsKey(r))
                    _factionDirs.Add(r,f);
                else
                    _factionDirs[r] = f;

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

        protected virtual IEnumerable<T> GetBlueprints<T>(WorldFactoryResource fi) where T:HasStatsBlueprint
        {
            try
            {
                return Compiler.Instance.Deserializer.Deserialize<List<T>>(fi.Content);
            }
            catch(Exception e)
            {
                throw new Exception($"Error loading {typeof(T).Name} in file {fi.Location}",e);
            }
        }
        protected virtual IEnumerable<DialogueNode> GetDialogue(WorldFactoryResource fi)
        {
                try
                {
                    var factory = new DialogueNodeFactory();

                    var blueprints = Compiler.Instance.Deserializer.Deserialize<DialogueNodeBlueprint[]>(fi.Content);

                    return blueprints.Select(b=>factory.Create(b));
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Error in dialogue yaml:{ e.Message } in file '{fi.Location}'",e);
                }
        }
    }
}
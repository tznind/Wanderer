using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLua;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Extensions;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Places;
using Wanderer.Plans;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer
{
    public class World : IWorld
    {
        public Map Map { get; set;} = new Map();

        public PlanningSystem PlanningSystem { get; set; } = new PlanningSystem();

        public HashSet<IActor> Population { get; set; } =  new HashSet<IActor>();
        public IRelationshipSystem Relationships { get; set; } = new RelationshipSystem();
        public IDialogueSystem Dialogue { get; set; } = new DialogueSystem();
        public IList<IInjurySystem> InjurySystems { get; set; } = new List<IInjurySystem>(new IInjurySystem[]
            {
                new TissueInjurySystem(),
                new HungerInjurySystem(),
                new FireInjurySystem()
            });
        public IList<INegotiationSystem> NegotiationSystems { get; set; } = new List<INegotiationSystem>(new []{new NegotiationSystem()});

        public IRoomFactory RoomFactory { get; set; }

        public Random R { get; set; } = new Random(100);

        [JsonIgnore]
        public You Player
        {
            get { return (You) Population.FirstOrDefault(p => p is You); }
        }

        public IFactionCollection Factions { get; set; } = new FactionCollection();

        public string ResourcesDirectory { get; set; } = Compiler.GetDefaultResourcesDirectory();

        /// <summary>
        /// Returns settings suitable for loading/saving worlds
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var config = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                Formatting = Formatting.Indented
            };

            return config;
        }

        private void RunNpcActions(ActionStack stack,IUserinterface ui)
        {
            //use ToArray because people might blow up rooms or kill one another
            foreach (var npc in Population.OfType<Npc>().OrderByDescending(a=>a.GetFinalStats()[Stat.Initiative]).ToArray())
            {
                //if occupant was killed by a previous action
                if(!Population.Contains(npc))
                    continue;
                
                //if npc is in an explored(ish) location
                if(!ShouldRunActionsIn(npc.CurrentLocation))
                    return;

                //if the npc has a cunning plan (and isn't being coerced)!
                if (npc.NextAction == null)
                {
                    //give the Npc a chance to form a plan
                    PlanningSystem.Apply(new SystemArgs(this,ui,0,null,npc,stack.Round));

                    //if the Npc has decided what to do
                    if (npc.Plan != null)
                    {
                        stack.RunStack(this,ui,npc.Plan,npc,Population.SelectMany(p=>p.GetFinalBehaviours()));
                        continue;
                    }

                }

                
                if(npc.Decide(ui, "Pick Action", null, out IAction chosen, npc.GetFinalActions().ToArray(), 0))
                    stack.RunStack(this,ui,chosen,npc,Population.SelectMany(p=>p.GetFinalBehaviours()));
            }
        }

        /// <summary>
        /// Returns true if events in the <paramref name="place"/> should be run (this could be useful to prevent exponential
        /// map scaling as npc move to new locations and discover friends who also wander off).
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        private bool ShouldRunActionsIn(IPlace place)
        {
            return
                place.IsExplored || place.GetPoint().Distance(Player.CurrentLocation.GetPoint()) <= 2;
        }

        /// <summary>
        /// Returns all behaviours as a new array (to allow modification in foreach)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBehaviour> GetAllBehaviours()
        {
            return Population.SelectMany(a => a.GetFinalBehaviours()).ToArray();
        }

        /// <inheritdoc/>
        public void RunRound(IUserinterface ui, IAction playerAction)
        {
            if (Player.Dead)
            {
                ui.ShowMessage("Dead","Alas you are too dead to do that");
                return;
            }

            //get fresh round logs
            ui.Log.RoundResults.Clear();

            var stack = new ActionStack();
            var actionRun = stack.RunStack(this,ui, playerAction, Player,GetAllBehaviours());

            if (actionRun)
            {
                RunNpcActions(stack,ui);

                foreach (IBehaviour b in GetAllBehaviours()) 
                    b.OnRoundEnding(this,ui, stack.Round);

                var results = GetPlayerVisibleLogResults(ui).ToArray();
                if(results.Any())
                    ui.ShowMessage("Round Results",string.Join('\n',results));

                ui.Refresh();
            }
        }

        /// <summary>
        /// Returns log entries near the Player
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        private IEnumerable<string> GetPlayerVisibleLogResults(IUserinterface ui)
        {
            var playerLocation = Player.CurrentLocation.GetPoint();
            return ui.Log.RoundResults.Where(r => r.Location == null || Math.Abs(r.Location.Distance(playerLocation)) < 0.01).Select(e=>e.Message);
        }

        public void Erase(IItem item)
        {
            foreach (var actor in Population)
                if (actor.Items.Contains(item))
                    actor.Items.Remove(item);
            
            foreach (var room in Map.Values)
                if (room.Items.Contains(item))
                    room.Items.Remove(item);

            item.IsErased = true;
        }

        public virtual IPlace GetNewRoom(Point3 newPoint)
        {
            var factionRooms = Factions.Select(f => f.RoomFactory)
                .Where(b => b.Blueprints.Any(b.Spawnable))
                .ToList();

            return factionRooms.Union(new[] {RoomFactory}).ToArray().GetRandom(R).Create(this);
        }

        public ISystem GetSystem(Guid g)
        {
            //TODO: This should return other systems too
            return InjurySystems.FirstOrDefault(i=>i.Identifier == g);
        }

    }
}

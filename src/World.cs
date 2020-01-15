using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer
{
    public class World : IWorld
    {
        public Map Map { get; set;} = new Map();

        public HashSet<IActor> Population { get; set; } =  new HashSet<IActor>();
        public IRelationshipSystem Relationships { get; set; } = new RelationshipSystem();
        public IDialogueSystem Dialogue { get; set; } = new DialogueSystem();

        public IRoomFactory RoomFactory { get; set; }

        public Random R { get; set; } = new Random(100);

        [JsonIgnore]
        public You Player
        {
            get { return (You) Population.FirstOrDefault(p => p is You); }
        }

        public IFactionCollection Factions { get; set; }

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
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            return config;
        }

        private void RunNpcActions(ActionStack stack,IUserinterface ui)
        {
            //use ToArray because people might blow up rooms or kill one another
            foreach (var npc in Population.OrderByDescending(a=>a.GetFinalStats()[Stat.Initiative]).ToArray())
            {
                //player always acts first in any round (e.g. in order to coerce people, cancel actions etc)
                if(npc is You)
                    continue;

                //if occupant was killed by a previous action
                if(!Population.Contains(npc))
                    continue;

                //if npc is in an explored location and decides to do an action
                if(ShouldRunActionsIn(npc.CurrentLocation)
                   && npc.Decide(ui, "Pick Action", null, out IAction chosen,
                    npc.GetFinalActions().ToArray(), 0))
                    stack.RunStack(ui,chosen,npc,Population.SelectMany(p=>p.GetFinalBehaviours()));
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

            var stack = new ActionStack();
            var actionRun = stack.RunStack(ui, playerAction, Player,GetAllBehaviours());

            if (actionRun)
            {
                RunNpcActions(stack,ui);

                foreach (IBehaviour b in GetAllBehaviours()) 
                    b.OnRoundEnding(ui, stack.Round);

                if(ui.Log.RoundResults.Any())
                    ui.ShowMessage("Round Results",string.Join('\n',GetPlayerVisibleLogResults(ui)));

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
    }
}

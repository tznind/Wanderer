﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;
using StarshipWanderer.UI;

namespace StarshipWanderer
{
    public class World : IWorld
    {
        public Map Map { get; set;} = new Map();

        public HashSet<IActor> Population { get; set; } =  new HashSet<IActor>();
        
        public IRoomFactory RoomFactory { get; set; }

        public Random R { get; set; } = new Random(100);

        [JsonIgnore]
        public You Player
        {
            get { return (You) Population.FirstOrDefault(p => p is You); }
        } 
        
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
                if(npc is You)
                    continue;

                //if occupant was killed by a previous action
                if(!Population.Contains(npc))
                    continue;

                if(npc.Decide(ui, "Pick Action", null, out IAction chosen,
                    npc.GetFinalActions().ToArray(), 0))
                    stack.RunStack(ui,chosen,npc,Population.SelectMany(p=>p.GetFinalBehaviours()));
            }
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
            var stack = new ActionStack();
            var actionRun = stack.RunStack(ui, playerAction, Player,GetAllBehaviours());

            if (actionRun)
            {
                RunNpcActions(stack,ui);

                foreach (IBehaviour b in GetAllBehaviours()) 
                    b.OnRoundEnding(ui, stack.Round);

                if(ui.Log.RoundResults.Any())
                    ui.ShowMessage("Round Results",string.Join('\n',ui.Log.RoundResults),false,Guid.Empty);
            }
        }
    }
}

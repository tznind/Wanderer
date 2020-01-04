using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actors
{
    public class Npc : Actor
    {

        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected Npc()
        {

        }

        public Npc(string name,IPlace currentLocation) : base( name,currentLocation)
        {
        }

        /// <summary>
        /// Forces the next action decided by <see cref="Npc"/> to be this.  After which it will be cleared
        /// </summary>
        public CoerceFrame NextAction { get; set; }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude)
        {
            //if we are being forced to perform an action
            if(typeof(IAction).IsAssignableFrom(typeof(T)))
                if (NextAction != null )
                {
                    //if it is the first time we are being asked what to do (action) after coercion
                    if (!NextAction.Chosen)
                    {
                        //pick the coerced act
                        chosen = (T) NextAction.CoerceAction;
                        NextAction.Chosen = true; //and mark that we are not going to pick it again
                        return true;
                    }
                    
                    //we have already attempted the coerced action, clear it
                    NextAction = null;
                }

            //if we are mid coercion we must let the coercer pick targets.
            //when picking targets the coercer should know the Attitude (how kind)
            //the action they are forcing is (in order to pick a good target)
            if (NextAction != null)
                return NextAction.PerformedBy.Decide(ui,title, body, out chosen, 
                    options,NextAction.CoerceAction.Attitude); 

            List<T> narrowOptions = new List<T>(options);
            
            //if it's something bad
            if (attitude < 0 && this is T npcAsT)
            {
                //don't pick yourself
                narrowOptions.Remove(npcAsT);
            }

            //If there are no options pick null return false
            if (!narrowOptions.Any())
            {
                chosen = default;
                return false;
            }
            
            //pick random option
            chosen = narrowOptions[CurrentLocation.World.R.Next(0, narrowOptions.Count)];

            //if picked option was default (e.g. None Enums) return false
            return !chosen.Equals(default(T));
        }

        public override void Kill(IUserinterface ui)
        {
            CurrentLocation.World.Population.Remove(this);

            foreach (IItem item in Items)
                CurrentLocation.Items.Add(item);

            //just in case dead actors somehow go somewhere
            Items.Clear();

        }
    }
}
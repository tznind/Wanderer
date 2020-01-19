using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

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
            BaseBehaviours.Add(new RelationshipFormingBehaviour(this));
        }

        /// <summary>
        /// Forces the next action decided by <see cref="Npc"/> to be this.  After which it will be cleared
        /// </summary>
        public CoerceFrame NextAction { get; set; }
        
        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, double attitude)
        {
            if (Dead)
            {
                chosen = default;
                return false;
            }

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
                    options,NextAction.Attitude); 

            List<T> narrowOptions = new List<T>(options);
            
            if (typeof(T) == typeof(IActor))
            {
                narrowOptions = DecideActor(narrowOptions.OfType<IActor>(), attitude).Cast<T>().ToList();
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

        public virtual IEnumerable<IActor> DecideActor(IEnumerable<IActor> input, double attitude)
        {
            foreach (var actor in input)
            {
                //don't pick yourself for bad actions
                if(attitude < 0 && actor == this)
                    continue;
                
                //or anyone you have a relationship with
                var attitudeTowardsActor =
                    CurrentLocation.World.Relationships.SumBetween(this, actor);
                
                //if it is hostile and do you hate them enough?
                if(attitude < 0 && attitudeTowardsActor > attitude)
                        continue;

                //if it is kind action but you don't like them enough to do it
                if(attitude > 0 && attitudeTowardsActor < attitude)
                    continue;

                yield return actor;
            }
        }

        public override void Kill(IUserinterface ui, Guid round, string reason)
        {
            if (!Dead)
            {
                ui.Log.Info(new LogEntry($"{this} died of {reason}", round, this));
                
                foreach (IItem item in Items.ToArray())
                    item.Drop(ui, this, round);

                foreach (var r in CurrentLocation.World.Relationships.ToArray())
                    r.HandleActorDeath(this);
            }
            
            Dead = true;
        }

    }
}
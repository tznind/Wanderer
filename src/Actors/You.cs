using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Actions.Coercion;
using Wanderer.Rooms;
using Wanderer.Stats;

namespace Wanderer.Actors
{
    /// <summary>
    /// The human player character.  All decisions triggered use the <see cref="IUserinterface"/>
    /// </summary>
    public class You : Actor
    {


        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected You()
        {

        }

        /// <summary>
        /// Creates a new instance of the player in the given <see cref="IRoom"/> (this includes adding them to the world population)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentLocation"></param>
        public You(string name, IRoom currentLocation):base( name,currentLocation)
        {
            BaseStats[Stat.Loyalty] = 10;
            BaseStats[Stat.Fight] = 20;
            BaseStats[Stat.Coerce] = 10;
            BaseStats[Stat.Savvy] = 20;

            //player can coerce Npc
            BaseActions.Add(new CoerceAction());
            //player can inspect Npc
            BaseActions.Add(new InspectAction());

            //for now lets not confuse the player by having Npc countermand their orders
            BaseActions.Add(new LeadershipAction());
        }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, double attitude)
        {
            if (!options.Any())
            {
                ui.ShowMessage("No Targets","There are no valid targets for that action");
                chosen = default;
                return false;
            }

            //ask user through UI
            return ui.GetChoice(title,body,out chosen,options);
        }

        public override void Move(IRoom newLocation)
        {
            base.Move(newLocation);
            newLocation.IsExplored = true;
        }

        public override void Kill(IUserinterface ui, Guid round, string reason)
        {

            //although you might get double dead in a round.  Only show the message once
            if (!Dead)
            {
                var narrative = new Narrative(this, "Dead",
                    $"You have tragically met your end.  Don't worry, many of your comrades will benefit from you sacrifice (at breakfast tomorrow).",$"You died of {reason}",round);
                narrative.Show(ui,true);
            }

            Dead = true;
        }

    }
}

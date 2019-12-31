using System;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
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
        /// Creates a new instance of the player in the given <see cref="IPlace"/> (this includes adding them to the world population)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentLocation"></param>
        public You(string name, IPlace currentLocation):base( name,currentLocation)
        {
            BaseStats[Stat.Loyalty] = 10;
            BaseStats[Stat.Fight] = 10;
            BaseStats[Stat.Coerce] = 10;

            //player can coerce Npc
            BaseActions.Add(new Coerce());
        }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude)
        {
            //ask user through UI
            return ui.GetChoice(title,body,out chosen,options);
        }
        
        public override void Kill(IUserinterface ui)
        {
            ui.ShowMessage("Dead","You have tragically met your end.  Don't worry, many of your comrades will benefit from you sacrifice (at breakfast tomorrow).",false,Guid.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Places;

namespace Wanderer.Systems
{
    /// <summary>
    /// Input arguments describing how and who to apply an <see cref="ISystem"/> to
    /// </summary>
    public class SystemArgs
    {
        /// <summary>
        /// The root game object
        /// </summary>
        public IWorld World { get; set; }

        /// <summary>
        /// The ui for logging events etc / showing messages
        /// </summary>
        public IUserinterface UserInterface;

        /// <summary>
        /// How strongly to apply the given system
        /// </summary>
        public double Intensity { get; set; } 

        /// <summary>
        /// The actor responsible for the application (can be null)
        /// </summary>
        public IActor AggressorIfAny { get; set; }

        /// <summary>
        /// The thing upon which to inflict the system
        /// </summary>
        public IHasStats Recipient { get; set; }

        /// <summary>
        /// The current round in which the system application is happening
        /// </summary>
        public Guid Round { get; set; }

        public SystemArgs(IWorld world,IUserinterface ui,double intensity,IActor aggressorIfAny,IHasStats recipient,Guid round)
        {
            World = world;
            Intensity = intensity;
            AggressorIfAny = aggressorIfAny;
            Recipient = recipient;
            Round = round;
            UserInterface = ui;
        }
        
        /// <summary>
        /// Returns the place the system args is occuring.  In rare occurrences this
        /// could be null 
        /// </summary>
        [JsonIgnore]
        public IPlace Place
        {
            get
            {
                IPlace place = AggressorIfAny?.CurrentLocation;

                if (place == null && Recipient is IActor a)
                    place = a.CurrentLocation;

                return place ?? Recipient as IPlace;
            }
        }
        
        ///<summary>
        /// Returns the final list of Actions associated with the Recipient
        ///
        /// <para>
        /// Returns final actions for the Recipient if they are an IActor. 
        /// Otherwise (if item or place) returns actions that AggressorIfAny (if not null) 
        /// could perform there. Otherwise returns the BaseActions of all children of 
        /// the Recipient
        /// </para>
        ///</summary>
        public IEnumerable<IAction> GetFinalActions()
        {
            if( Recipient is IActor actor)
                return actor.GetFinalActions();

            if(AggressorIfAny != null)
                return Recipient.GetFinalActions(AggressorIfAny);

            return Recipient.GetAllHaves().SelectMany(h=>h.BaseActions);
        }


        /// <summary>
        /// Returns the relationship as the <see cref="Recipient"/> observes the
        /// <see cref="AggressorIfAny"/> (if both are <see cref="IActor"/> - otherwise
        /// 0)
        /// </summary>
        public double Relationship =>
            AggressorIfAny != null && Recipient is IActor r
                ?
                AggressorIfAny.CurrentLocation.World.Relationships.SumBetween(r,AggressorIfAny)
                : 0;
    }
}
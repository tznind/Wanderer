using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer.Systems
{
    /// <summary>
    /// Input arguments describing how and who to apply an <see cref="ISystem"/> to
    /// </summary>
    public class SystemArgs
    {
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

        public SystemArgs(IUserinterface ui,double intensity,IActor aggressorIfAny,IHasStats recipient,Guid round)
        {
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
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using StarshipWanderer.Actors;

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
        public int Intensity { get; set; } 

        /// <summary>
        /// The actor responsible for the application (can be null)
        /// </summary>
        public IActor AggressorIfAny { get; set; }

        /// <summary>
        /// The actor upon which to inflict the system
        /// </summary>
        public IActor Recipient { get; set; }

        /// <summary>
        /// The current round in which the system application is happening
        /// </summary>
        public Guid Round { get; set; }

        public SystemArgs(IUserinterface ui,int intensity,IActor aggressorIfAny,IActor recipient,Guid round)
        {
            Intensity = intensity;
            AggressorIfAny = aggressorIfAny;
            Recipient = recipient;
            Round = round;
            UserInterface = ui;
        }
    }
}
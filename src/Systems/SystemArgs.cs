using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;

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
        public IUserinterface UserInterface { get; set; }

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
        public IRoom Room
        {
            get
            {
                IRoom place = AggressorIfAny?.CurrentLocation;

                if (place == null && Recipient is IActor a)
                    place = a.CurrentLocation;

                return place ?? Recipient as IRoom;
            }
        }

        /// <summary>
        /// Returns all items owned by the Recipient (if it is an actor or place). Otherwise
        /// returns empty
        /// </summary>
        [JsonIgnore]
        public IEnumerable<IItem> Items
        {
            get
            {
                if (Recipient is IActor a)
                    return a.Items;

                if(Recipient is IRoom p)
                    return p.Items;

                return new IItem[0];
            }
        }
        
        /// <summary>
        /// Returns a reference to the <paramref name="target"/>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public IHasStats GetTarget(SystemArgsTarget target)
        {
            switch (target)
            {
                case SystemArgsTarget.Aggressor:
                    return AggressorIfAny ?? Recipient;
                case SystemArgsTarget.Recipient:
                    return Recipient;
                case SystemArgsTarget.Room:
                    return Room;
                case SystemArgsTarget.Owner:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}:AggressorIfAny={AggressorIfAny?.ToString() ?? "Null"},Recipient={Recipient?.ToString() ?? "Null"}";
        }
    }
}
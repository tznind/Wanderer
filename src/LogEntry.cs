using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    /// <summary>
    /// An event in the game world that should be logged.  May be associated with a specific
    /// location (in order to filter user log only to nearby events).
    /// </summary>
    public class LogEntry
    {
        public string Message { get; }
        public Guid Round { get; }
        public Point3 Location { get; }

        public LogEntry(string message)
        {
            Message = message;
            Round = Guid.Empty;
        }

        public LogEntry(string message,Guid round,Point3 location)
        {
            Message = message;
            Round = round;
            Location = location;
        }

        public LogEntry(string message, Guid round, IActor actor):this(message,round,actor.CurrentLocation.GetPoint())
        {
            
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
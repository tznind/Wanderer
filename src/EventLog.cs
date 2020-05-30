using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Targets;

namespace Wanderer
{
    /// <summary>
    /// Stores messages generated during the game.  Supports reporting to NLog as well as capturing discrete rounds for reporting to the UI etc.  Also supports location based messages e.g. to prevent the player knowing something has happened
    /// </summary>
    public class EventLog
    {
        public MemoryTarget Target;
        private Logger _log;

        /// <summary>
        /// Sets up the log using your current NLog configuration (if any) and initializes the in memory <see cref="Target"/>
        /// </summary>
        public void Register()
        {
            var config = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            Target = new MemoryTarget {Layout = "${message}"};

            // Message format
            config.AddRuleForAllLevels(Target);
            LogManager.Configuration = config;

            _log = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// All activities that happened since the last round began
        /// </summary>
        public List<LogEntry> RoundResults { get; } = new List<LogEntry>();
        private Guid _currentRound;
        
        /// <summary>
        /// Record an event in the world
        /// </summary>
        /// <param name="toLog"></param>
        public void Info(LogEntry toLog)
        {
            //if a new round has begun clear last rounds log and start again
            if (toLog.Round != Guid.Empty && toLog.Round != _currentRound)
            {
                RoundResults.Clear();
                _currentRound = toLog.Round;
            }

            if(toLog.Round != Guid.Empty)
                RoundResults.Add(toLog);

            _log?.Info(toLog.Message);
        }

        /// <summary>
        /// Removes all in memory log entries
        /// </summary>
        public void Clear()
        {
            Target.Logs.Clear();
            RoundResults.Clear();
        }

    }
}

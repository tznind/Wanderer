using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Targets;

namespace StarshipWanderer
{
    public class EventLog
    {
        public readonly MemoryTarget Target = new MemoryTarget();
        private Logger _log;

        public void Register()
        {
            Target.Layout = "${message}";
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(Target, LogLevel.Debug);

            _log = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// All activities that happened since the last round began
        /// </summary>
        public List<LogEntry> RoundResults { get; } = new List<LogEntry>();
        private Guid _currentRound;
        
        public void Info(LogEntry toLog)
        {
            //if a new round has begun clear last rounds log and start again
            if (toLog.Round != Guid.Empty && toLog.Round != _currentRound)
            {
                RoundResults.Clear();
                _currentRound = toLog.Round;
            }

            RoundResults.Add(toLog);
            _log.Info(toLog.Message);
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

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
        public List<string> RoundResults { get; } = new List<string>();
        private Guid _currentRound;

        public void Info(string s,Guid round)
        {
            //if a new round has begun clear last rounds log and start again
            if (round != Guid.Empty && round != _currentRound)
            {
                RoundResults.Clear();
                _currentRound = round;
            }

            RoundResults.Add(s);
            _log.Info(s);
        }
    }
}

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

        public void Info(string s)
        {
            _log.Info(s);
        }
    }
}

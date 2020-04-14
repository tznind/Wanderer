using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer;

namespace Tests
{
    class EventLogTests
    {
        [Test]
        public void TestBasicConstructAndUse()
        {
            var log = new EventLog();
            log.Info(new LogEntry("something happened",Guid.Empty, new Point3(0,0,0)));
        }
    }
}

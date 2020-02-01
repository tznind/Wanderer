using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace Tests
{
    class LogEntryTests : UnitTest
    {
        [Test]
        public void TestLogEntry()
        {
            var ui = GetUI();

            ui.Log.Info(new LogEntry("test2",Guid.NewGuid(),new Point3(0,0,0)));
            Assert.AreEqual("test2",ui.Log.RoundResults.Single().Message);

            ui.Log.Info(new LogEntry("test3",Guid.NewGuid(),new Point3(0,0,0)));
            Assert.AreEqual("test3",ui.Log.RoundResults.Single().Message);
            
            ui.Log.Info(new LogEntry("test4",Guid.NewGuid(),
                Mock.Of<IActor>(a=> a.CurrentLocation == Mock.Of<IPlace>(p=>p.GetPoint() == new Point3(1,2,3)))));
            Assert.AreEqual("test4",ui.Log.RoundResults.Single().Message);
        }
    }
}

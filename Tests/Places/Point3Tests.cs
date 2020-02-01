using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using TB.ComponentModel;

namespace Tests.Places
{
    class Point3Tests : UnitTest
    {
        [Test]
        public void Test_Distance()
        {
            Assert.AreEqual(1,new Point3(0, 0, 0).Distance(new Point3(0, 1, 0)));
            Assert.AreEqual(2, new Point3(0, 0, 0).Distance(new Point3(0, 2, 0)));

            Assert.IsTrue(Math.Abs(3.74 - new Point3(1, 1, 1).Distance(new Point3(2, 3, 4))) < 0.01);

            Assert.IsTrue(Math.Abs(46.50d - new Point3(-11, -22, -33).Distance(new Point3(2, 3, 4))) < 0.01);
        }

        [Test]
        public void TestPoint_ConvertibleTo_String()
        {
            var p = new Point3(1, 2, 3);

            Assert.IsTrue(p.IsConvertibleTo(typeof(string)));
            Assert.AreEqual("1,2,3", p.To(typeof(string)));
            Assert.IsFalse(p.IsConvertibleTo<int>());

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer;
using TB.ComponentModel;

namespace Tests.Places
{
    class Point3Tests : UnitTest
    {

        [Test]
        public void TestEquality()
        {
            
            var p = new Point3(1, 2, 3);
            var p2 = new Point3(1, 2, 3);

            Assert.AreEqual(p,p2);
            Assert.AreEqual(p.GetHashCode(),p2.GetHashCode());

            Assert.IsFalse(p.Equals(null));
            Assert.IsTrue(p.Equals(p));
            Assert.IsFalse(p.Equals(1));
        }

        [Test]
        public void Test_Offset()
        {
            var p = new Point3(1, 2, 3);
            
            Assert.AreEqual(new Point3(1,2,4),p.Offset(Direction.Up, 1));
            Assert.AreEqual(new Point3(1,2,2),p.Offset(Direction.Down, 1));
            Assert.AreEqual(new Point3(1,3,3),p.Offset(Direction.North, 1));
            Assert.AreEqual(new Point3(1,1,3),p.Offset(Direction.South, 1));
            Assert.AreEqual(new Point3(2,2,3),p.Offset(Direction.East, 1));
            Assert.AreEqual(new Point3(0,2,3),p.Offset(Direction.West, 1));

            Assert.Throws<ArgumentException>(()=>p.Offset(Direction.None, 1));

            Assert.AreEqual(new Point3(2,4,6),p.Offset(new Point3(1,2,3)));

        }

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

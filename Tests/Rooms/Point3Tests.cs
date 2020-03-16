using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;

namespace Tests.Rooms
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
        public void TestConversion()
        {
            var converter = new Point3Converter();

            Assert.IsFalse(converter.CanConvertFrom(Mock.Of<ITypeDescriptorContext>(),typeof(int)));
            Assert.IsTrue(converter.CanConvertFrom(Mock.Of<ITypeDescriptorContext>(),typeof(string)));


            Assert.AreEqual(new Point3(1,2,3),converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(),CultureInfo.CurrentCulture, "1,2,3"));
            Assert.Throws<NotSupportedException>(()=>converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(),CultureInfo.CurrentCulture,5555));

            Assert.AreEqual("3,2,1",converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(),CultureInfo.CurrentCulture, new Point3(3,2,1),typeof(string)));
            Assert.Throws<NotSupportedException>(()=>converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(),CultureInfo.CurrentCulture, new Point3(3,2,1),typeof(int)));
        }
    }
}

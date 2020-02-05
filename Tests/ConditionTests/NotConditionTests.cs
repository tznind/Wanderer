using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace Tests.ConditionTests
{
    class NotConditionTests
    {
        [Test]
        public void TestNot_IActor()
        {
            var a = new AlwaysCondition<IActor>();
            Assert.IsTrue(a.IsMet(Mock.Of<IActor>()));

            var not = new Not<IActor>(a);
            Assert.IsFalse(not.IsMet(Mock.Of<IActor>()));
        }

        [Test]
        public void TestNot_Has()
        {
            var i1 =  new Not<IHasStats>(new Has<Light>(true));

            var i2 = Not<object>.Decorate(new Has<Light>(true));

            Assert.AreEqual(i1.GetType(),i2.GetType());
        }
        
        [Test]
        public void TestNot_Decorate()
        {
            var a = new AlwaysCondition<IActor>();
            Assert.IsTrue(a.IsMet(Mock.Of<IActor>()));

            var not = Not<object>.Decorate(a);
            Assert.IsFalse(((ICondition<IActor>)not).IsMet(Mock.Of<IActor>()));
        }
    }
}

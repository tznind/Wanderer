using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace Tests.Actions
{
    class FightTests  : UnitTest
    {
        [Test]
        public void Test_NoTargets()
        {
            var you = YouInARoom(out IWorld w);

            var f = new FightAction();

            Assert.IsFalse(f.HasTargets(you));

            var other = new Npc("ff", you.CurrentLocation);

            Assert.IsTrue(f.HasTargets(you));
        }

    }
}

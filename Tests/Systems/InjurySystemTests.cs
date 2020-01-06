using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Adjectives.ActorOnly;
using StarshipWanderer.Systems;
using Tests.Actions;
using Enumerable = System.Linq.Enumerable;

namespace Tests.Systems
{
    class InjurySystemTests
    {
        [Test]
        public void Test_AllInjuriesUnique()
        {
            List<string> seenSoFar = new List<string>();

            var sys = new InjurySystem();

            HashSet<IAdjective> adjectives = new HashSet<IAdjective>();

            var a = Mock.Of<IActor>(b=>b.Adjectives == adjectives);

            for (int i = 0; i < 60; i++)
            {
                adjectives.Clear();
                sys.Apply(new SystemArgs(M.UI_GetChoice(new object()),i,null,a,Guid.Empty));

                //it should have applied 1 injury
                Assert.AreEqual(1,adjectives.Count);
                var injury = (Injured)adjectives.Single();

                Assert.AreNotEqual(InjuryRegion.None,injury);

                var newInjury = injury.Name;
                Assert.IsFalse(seenSoFar.Contains(newInjury),$"Saw duplicate injury '{newInjury}' for i={i}");
                seenSoFar.Add(newInjury);
            }
        }
    }
}

using NUnit.Framework;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Stats;

namespace Tests.Adjectives
{
    class AdjectiveTests
    {
        [Test]
        public void TestAttractive()
        {
            var d  = new Actor("Dave");
            
            d.BaseStats[Stat.Fight] = 20;
            d.BaseStats[Stat.Coerce] = 20;

            Assert.AreEqual(20,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

            var attractive = new Attractive(d);
            d.Adjectives.Add(attractive);

            Assert.AreEqual(35,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

            var injury = new Injured(d);
            d.Adjectives.Add(injury);

            //now injured
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(10,d.GetFinalStats()[Stat.Fight]);

            d.Adjectives.Remove(injury);
            
            Assert.AreEqual(35,d.GetFinalStats()[Stat.Coerce]);
            Assert.AreEqual(20,d.GetFinalStats()[Stat.Fight]);

        }
    }
}

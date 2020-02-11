using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Compilation;
using StarshipWanderer.Conditions;
using StarshipWanderer.Items;
using YamlDotNet.Core;

namespace Tests.CompilerTests
{
    public class ConstructorCollectionTests
    {
        [Test]
        public void TestConstruct_Explicit()
        {
            var con = new ConstructorCollection(typeof(Npc));

            //public constructors only
            Assert.AreEqual(1,con.Count);
            Assert.AreEqual(typeof(Npc),con.Type);
        }
        [Test]
        public void TestConstruct_ByName()
        {
            var con = new ConstructorCollection(
                Compiler.Instance.TypeFactory.Create(typeof(IActor),true,true),
                "Npc",null);

            Assert.AreEqual(1,con.Count);
            Assert.AreEqual(typeof(Npc),con.Type);
        }
    }
}
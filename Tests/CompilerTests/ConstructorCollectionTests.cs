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

        
        [Test]
        public void TestConstruct_GenericByName()
        {
            var con = new ConstructorCollection(
                //when we are trying to build a condition which can apply to the generic case
                Compiler.Instance.TypeFactory.Create(typeof(ICondition<IHasStats>),true,true),

                //And we know the specific Type class name.  And template
                "StatCondition",new[] {"IActor"});

            Assert.AreEqual(1,con.Count);

            //System can find our class
            Assert.AreEqual(typeof(StatCondition<IActor>),con.Type);
        }
        
        [Test]
        public void TestConstruct_GenericWithMissingTemplate()
        {
            
            var ex = Assert.Throws<ParseException>(()=>new ConstructorCollection(
                //when we are trying to build a condition which can apply to the generic case
                Compiler.Instance.TypeFactory.Create(typeof(ICondition<IHasStats>),true,true),

                //And we know the specific Type class name.  And template
                "StatCondition",null));
            
            Assert.AreEqual(
                @"Could not find StarshipWanderer.Conditions.ICondition`1[StarshipWanderer.IHasStats] called StatCondition.Did you mean:StatCondition<T1>",
                ex.Message);
        }

        
        
        [Test]
        public void TestConstruct_NonExistantType()
        {
            
            var ex = Assert.Throws<ParseException>(()=>new ConstructorCollection(
                //when we are trying to build a condition which can apply to the generic case
                Compiler.Instance.TypeFactory.Create(typeof(ICondition<IHasStats>),true,true),

                //And we know the specific Type class name.  And template
                "StatConditionnnnn",null));
            
            Assert.AreEqual(
                @"Could not find StarshipWanderer.Conditions.ICondition`1[StarshipWanderer.IHasStats] called StatConditionnnnn.",
                ex.Message);
        }

    }
}
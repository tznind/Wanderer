using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Plans;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Tests.Actions
{
    class LeadershipTests : UnitTest
    {
        [Test]
        public void TestLeadership_NoTargets()
        {
            var you = YouInARoom(out IWorld w);

            //nobody else in room
            Assert.IsFalse(new LeadershipAction().HasTargets(you));

            //enemy in room
            var npc = new Npc("Chaos Bob",you.CurrentLocation);

            //he doesnt like you
            var rel = new PersonalRelationship(npc,you){Attitude = -10};
            w.Relationships.Add(rel);

            Assert.IsFalse(new LeadershipAction().HasTargets(you));

            //he is your friend
            rel.Attitude = 10;
            Assert.IsTrue(new LeadershipAction().HasTargets(you));
        }


        [Test]
        public void TestLeadership_PrioritizePlan()
        {
            TwoInARoomWithRelationship(10,false,out You you, out IActor them,out IWorld world);

            // they must have the actions for the plans to be viable
            them.BaseActions.Add(new LoadGunsAction());
            them.BaseActions.Add(new InspectAction());

            var plan1 = new Plan()
                {
                    Weight = 10,
                    Do = Mock.Of<IFrameSource>(f=>f.GetFrame(It.IsAny<SystemArgs>()) == new Frame(them,new InspectAction(),0))
                };
            var plan2 = new Plan()
                {
                    Weight = 0,
                    Do = Mock.Of<IFrameSource>(f=>f.GetFrame(It.IsAny<SystemArgs>()) == new Frame(them,new LoadGunsAction(),0))
                };

                Assert.IsNull(((Npc)them).Plan);

            world.PlanningSystem.Plans.Add(plan2);
            world.PlanningSystem.Plans.Add(plan1);
            world.PlanningSystem.Apply(new SystemArgs(world,GetUI(),0,null,them,Guid.Empty));

            //they pick the highest weighted plan
            Assert.IsInstanceOf(typeof(InspectAction),((Npc)them).Plan.Action);

            Assert.IsEmpty(them.Adjectives.OfType<LedAdjective>().ToArray());

            //now give them some friendly leadership advice to go with plan2 instead
            world.RunRound(GetUI(them,plan2,30d),new LeadershipAction());

            //they should now be being led by your guidance
            Assert.AreEqual(1,them.Adjectives.OfType<LedAdjective>().Count());
            
            //get them to make a decision
            world.PlanningSystem.Apply(new SystemArgs(world,GetUI(),0,null,them,Guid.Empty));

            //they should now be picking your plan instead
            Assert.IsInstanceOf(typeof(LoadGunsAction),((Npc)them).Plan.Action);

        }

    }
}

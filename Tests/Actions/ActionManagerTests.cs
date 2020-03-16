using System.Linq;
using NUnit.Framework;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Stats;

namespace Tests.Actions
{

    public class ActionManagerTests : UnitTest
    {
        [Test]
        public void ActionManagerTest_IgnoreTargetRequirement()
        {
            var you = YouInARoom(out _);

            var manager = new ActionManager();


            you.BaseActions.Clear();
            you.Items.Add(new Item("Rusty Hook")
            {
                BaseActions = {new FightAction().With(Stat.Fight,10)}
            });
            
            you.Items.Add(new Item("Plank of Wood")
            {
                BaseActions = {new FightAction().With(Stat.Fight,5)}
            });
            
            Assert.AreEqual("Fight",manager.GetTypes(you,false).Single().Name);
            Assert.AreEqual(2,manager.GetInstances(you,manager.GetTypes(you,false).Single(),false).Count);
        }

        [Test]
        public void ActionManagerTest_MustHaveTarget()
        {
            var you = YouInARoom(out _);

            var manager = new ActionManager();
            
            you.BaseActions.Clear();
            you.Items.Add(new Item("Rusty Hook")
            {
                BaseActions = {new FightAction().With(Stat.Fight,10)}
            });
            
            you.Items.Add(new Item("Plank of Wood")
            {
                BaseActions = {new FightAction().With(Stat.Fight,5)}
            });
            
            Assert.IsEmpty(manager.GetTypes(you,true));
            Assert.IsEmpty(manager.GetInstances(you,manager.GetTypes(you,true).SingleOrDefault(),true));

            //now that there is someone in your room you can fight
            var dummy = new Npc("TargetDummy", you.CurrentLocation);

            Assert.IsNotEmpty(manager.GetTypes(you,true));
            Assert.IsNotEmpty(manager.GetInstances(you,manager.GetTypes(you,true).SingleOrDefault(),true));
        }
    }
}

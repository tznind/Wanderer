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

            var hook = new Item("Rusty Hook");
            hook.BaseActions.Clear();
            hook.BaseActions.Add(new FightAction(hook).With(Stat.Fight,10));
            var wood = new Item("Plank of Wood");
            wood.BaseActions.Clear();
            wood.BaseActions.Add(new FightAction(wood).With(Stat.Fight,5));

            you.Items.Add(hook);
            you.Items.Add(wood);
            
            Assert.AreEqual("Fight",manager.GetTypes(you,false).Single().Name);
            Assert.AreEqual(2,manager.GetInstances(you,manager.GetTypes(you,false).Single(),false).Count);
        }

        [Test]
        public void ActionManagerTest_MustHaveTarget()
        {
            var you = YouInARoom(out _);

            var manager = new ActionManager();
            
            you.BaseActions.Clear();

            var hook = new Item("Rusty Hook");
            hook.BaseActions.Clear();
            hook.BaseActions.Add(new FightAction(hook).With(Stat.Fight,10));

            var wood = new Item("Plank of Wood");
            wood.BaseActions.Clear();
            wood.BaseActions.Add(new FightAction(wood).With(Stat.Fight,5));
            
            
            you.Items.Add(hook);
            you.Items.Add(wood);

            Assert.IsEmpty(manager.GetTypes(you,true));
            Assert.IsEmpty(manager.GetInstances(you,manager.GetTypes(you,true).SingleOrDefault(),true));

            //now that there is someone in your room you can fight
            var dummy = new Npc("TargetDummy", you.CurrentLocation);

            Assert.IsNotEmpty(manager.GetTypes(you,true));
            Assert.IsNotEmpty(manager.GetInstances(you,manager.GetTypes(you,true).SingleOrDefault(),true));
        }
    }
}

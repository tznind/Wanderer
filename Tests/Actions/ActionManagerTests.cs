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
            
            Assert.Contains("Fight",manager.GetTypes(you,false).Select(a=>a.Name).ToArray());
            Assert.AreEqual(2,manager.GetInstances(you,manager.GetTypes(you,false).Where(t=>t.Name == "Fight").Single(),false).Count);
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

            Assert.IsEmpty(manager.GetTypes(you,true).Where(a=>a.Name.Equals("Fight")));

            //now that there is someone in your room you can fight
            var dummy = new Npc("TargetDummy", you.CurrentLocation);

            Assert.Contains("Fight",manager.GetTypes(you,true).Select(a=>a.Name).ToArray());
            Assert.AreEqual(2,manager.GetInstances(you,manager.GetTypes(you,true).Where(t=>t.Name == "Fight").Single(),false).Count);
        }
    }
}

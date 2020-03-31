using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Items;
using Wanderer.Relationships;
using Wanderer.Stats;

namespace Tests.Actions
{
    class FightTests  : UnitTest
    {
        [Test]
        public void Test_NoTargets()
        {
            var you = YouInARoom(out IWorld w);

            var f = new FightAction(you);

            Assert.IsFalse(f.HasTargets(you));

            var other = new Npc("ff", you.CurrentLocation);

            Assert.IsTrue(f.HasTargets(you));
        } 
        
        [Test]
        public void Test_GangingUp()
        {
            var you = YouInARoom(out IWorld w);
            you.BaseStats[Stat.Fight] = 20;

            var a = new Npc("a", you.CurrentLocation);
            var b = new Npc("b", you.CurrentLocation);

            foreach (var enemy in new []{ a,b})
            {
                //they hate you a lot
                w.Relationships.Add(new PersonalRelationship(enemy,you)
                {
                    Attitude = -500
                });

                //and all they can do is fight!
                enemy.BaseActions.Clear();
                enemy.BaseActions.Add(new FightAction(you));
            }

            var ui = GetUI();
            
            w.RunRound(ui,new LoadGunsAction(you));

            Assert.IsTrue(ui.MessagesShown.Any(m=>m.Contains("a fought Test Wanderer")));
            Assert.IsTrue(ui.MessagesShown.Any(m=>m.Contains("b fought Test Wanderer")));

            Assert.AreEqual(2,you.Adjectives.Count(j=>j.Name.Equals("Tired")));

            a.Kill(ui,Guid.Empty,"Meteor");
            b.Kill(ui,Guid.Empty,"Meteor");
            w.RunRound(GetUI(),new LoadGunsAction(you));
            
            //should have worn off
            Assert.IsEmpty(you.Adjectives.Where(a=>a.Name.Equals("Tired")).ToArray());

        }

        [Test]
        public void Test_ActionStats()
        {
            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Fight] = 20;

            var weapon = new Item("Deadly Weapon");
            weapon.BaseActions.Add(new FightAction(weapon));
            weapon.BaseActions.Single().BaseStats.Increase(Stat.Fight, 33);
            you.Items.Add(weapon);

            var a = new Npc("a", you.CurrentLocation);
            a.BaseActions.Clear();
            var b = new Npc("b", you.CurrentLocation);
            b.BaseActions.Clear();

            RunRound(world,"Fight [Fists]",a);

            //you injury them a bit
            Assert.AreEqual(40,
                (a.Adjectives.OfType<IInjured>().Single()).Severity);

            //get rid of tired or any return injuries
            you.Adjectives.Clear();

            RunRound(world,"Fight [Deadly Weapon]",b);

            //you injury them a lot more
            Assert.AreEqual(73,
                (b.Adjectives.OfType<IInjured>().Single()).Severity);

        }
    }
}

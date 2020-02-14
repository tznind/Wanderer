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

            var f = new FightAction();

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
                enemy.BaseActions.Add(new FightAction());
            }

            var ui = GetUI();
            
            w.RunRound(ui,new LoadGunsAction());

            Assert.IsTrue(ui.MessagesShown.Any(m=>m.Contains("a fought Test Wanderer")));
            Assert.IsTrue(ui.MessagesShown.Any(m=>m.Contains("b fought Test Wanderer")));

            Assert.AreEqual(2,you.Adjectives.OfType<Tired>().Count());

            a.Kill(ui,Guid.Empty,"Meteor");
            b.Kill(ui,Guid.Empty,"Meteor");
            w.RunRound(GetUI(),new LoadGunsAction());
            
            //should have worn off
            Assert.IsEmpty(you.Adjectives.OfType<Tired>().ToArray());

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Adjectives;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Tests.Systems
{
    class TestResistances : UnitTest
    {
        [Test]
        public void TestResistancesByName()
        {
            var res = new Resistances();
            res.Immune.Add("Invincible");

            res.Resist.Add("Tough");
            res.Vulnerable.Add("Frail");
            
            var item = new Item("Apron");

            //this apron does not help at all.  The effect multiplier is 1 i.e. no change
            Assert.AreEqual(1,res.Calculate(item));

            item.Adjectives.Add(new Adjective(item){Name ="Invincible"});
            Assert.AreEqual(0,res.Calculate(item),"You should no longer be affected being invincible");

            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(item));

            item.Adjectives.Add(new Adjective(item){Name ="Tough"});
            Assert.AreEqual(0.5,res.Calculate(item), "Tough should decrease susceptibility");
            
            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(item));

            item.Adjectives.Add(new Adjective(item){Name ="Frail"});
            Assert.AreEqual(2,res.Calculate(item),"Frail should increase susceptibility");

            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(item));

            item.Adjectives.Add(new Adjective(item){Name ="Frail"});
            item.Adjectives.Add(new Adjective(item){Name ="Tough"});
            Assert.AreEqual(1,res.Calculate(item),"Frail and Tough should cancel out");

        }

        [Test]
        public void TestResistancesForActor()
        {
            var you = YouInARoom(out _);
            
            var res = new Resistances();
            res.Immune.Add("Invincible");

            res.Resist.Add("Tough");
            res.Vulnerable.Add("Frail");
            
            var item = new Item("Apron");
            you.Items.Add(item);

            //this apron does not help at all.  The effect multiplier is 1 i.e. no change
            Assert.AreEqual(1,res.Calculate(you,true));
            Assert.AreEqual(1,res.Calculate(you,false));

            item.Adjectives.Add(new Adjective(item){Name ="Invincible"});
            Assert.AreEqual(0,res.Calculate(you,true),"You should no longer be affected being invincible");
            Assert.AreEqual(1,res.Calculate(you,false),"If not including items it should be normal i.e. 1");

            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(you,true));

            item.Adjectives.Add(new Adjective(item){Name ="Tough"});
            Assert.AreEqual(0.5,res.Calculate(you,true), "Tough should decrease susceptibility");
            Assert.AreEqual(1,res.Calculate(you,false),"If not including items it should be normal i.e. 1");

            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(you,true));

            item.Adjectives.Add(new Adjective(item){Name ="Frail"});
            Assert.AreEqual(2,res.Calculate(you,true),"Frail should increase susceptibility");
            Assert.AreEqual(1,res.Calculate(you,false),"If not including items it should be normal i.e. 1");

            item.Adjectives.Clear();
            Assert.AreEqual(1,res.Calculate(you,true));

            item.Adjectives.Add(new Adjective(item){Name ="Frail"});
            item.Adjectives.Add(new Adjective(item){Name ="Tough"});
            Assert.AreEqual(1,res.Calculate(you,true),"Frail and Tough should cancel out");
            Assert.AreEqual(1,res.Calculate(you,false),"If not including items it should be normal i.e. 1");
        }
    }
}

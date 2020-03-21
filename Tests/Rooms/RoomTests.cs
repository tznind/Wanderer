using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Adjectives;
using Wanderer.Items;

namespace Tests.Rooms
{
    class RoomTests : UnitTest
    {

        [Test]
        public void TestGetAllHaves_Room()
        {
            Guid g1 = new Guid();
            Guid g2 = new Guid();
            Guid g3 = new Guid();

            var you = YouInARoom(out IWorld _);

            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g1));
            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g2));
            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g3));
            
            you.Adjectives.Add( new Adjective(you){Identifier = g2});

            you.Items.Add(new Item("orb") 
            {
                Identifier = g1,

            });

            you.Items.Single().Adjectives.Add( new Adjective(you){Identifier = g3});

            Assert.Contains(g1,you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).ToArray());
            Assert.Contains(g2,you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).ToArray());
            Assert.Contains(g3,you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).ToArray());

            you.Dead = true;

            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g1));
            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g2));
            Assert.IsFalse(you.CurrentLocation.GetAllHaves().Select(h=>h.Identifier).Contains(g3));

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace Tests.Relationships
{
    public class FactionRelationshipTests : UnitTest
    {
        IPlace _room;
        Faction _cops;
        Faction _robbers;
        Npc _cop1;
        Npc _cop2;
        Npc _robber1;
        Npc _robber2;
        private IWorld _world;
        private Npc _turncoat;
        private Npc _facelessMan;

        [SetUp]
        public void SetUp()
        {
            _room = InARoom(out _world);

            _cops = new Faction("Cops", FactionRole.Establishment);
            _robbers = new Faction("Robbers", FactionRole.Opposition);

            _cop1 = new Npc("Cop 1", _room);
            _cop1.FactionMembership.Add(_cops);

            _cop2 = new Npc("Cop 2",_room);
            _cop2.FactionMembership.Add(_cops);

            _robber1 = new Npc("Robber 1",_room);
            _robber1.FactionMembership.Add(_robbers);

            _robber2 = new Npc("Robber 2",_room);
            _robber2.FactionMembership.Add(_robbers);
            
            _turncoat = new Npc("Turncoat",_room);
            _turncoat.FactionMembership.Add(_robbers);
            _turncoat.FactionMembership.Add(_cops);

            _facelessMan = new Npc("Faceless Man",_room);

        }
        
        [Test]
        public void TestRelationshipStrength_WithinFaction()
        {
            _world.Relationships.Add(new IntraFactionRelationship(_cops, 10));

            Assert.AreEqual(0,GetTotalFor(_robber1, _robber2));
            Assert.AreEqual(10,GetTotalFor(_cop1, _cop2));

            Assert.AreEqual(10,GetTotalFor(_turncoat, _cop2));
            Assert.AreEqual(0,GetTotalFor(_turncoat, _robber1));

            Assert.AreEqual(10,GetTotalFor(_cop2,_turncoat));
            //todo : think about this one, why do they hate you just because your a cop eh?
            Assert.AreEqual(10,GetTotalFor(_robber1,_turncoat));

            Assert.AreEqual(0,GetTotalFor(_robber1, _facelessMan));
            Assert.AreEqual(0,GetTotalFor(_cop1, _facelessMan));
        }

        [Test]
        public void TestRelationshipStrength_AgainstOutsiders()
        {
            //robbers hate non robbers
            _world.Relationships.Add(new ExtraFactionRelationship(_robbers, 5));

            //but cops hate non cops more
            _world.Relationships.Add(new ExtraFactionRelationship(_cops, 7));

            //cops and robbers are all ok with their friends
            Assert.AreEqual(0,GetTotalFor(_robber1, _robber2));
            Assert.AreEqual(0,GetTotalFor(_cop1, _cop2));
       
            //todo: have to think about this one
            Assert.AreEqual(5,GetTotalFor(_turncoat, _cop2));
            Assert.AreEqual(7,GetTotalFor(_turncoat, _robber1));
            
            Assert.AreEqual(0,GetTotalFor(_cop2,_turncoat ));
            Assert.AreEqual(0,GetTotalFor(_robber1,_turncoat));

            Assert.AreEqual(5,GetTotalFor(_robber1, _cop1));
            Assert.AreEqual(7,GetTotalFor(_cop1,_robber2 ));

            //everyone hates the faceless man because he is in nobodies faction :(
            Assert.AreEqual(5,GetTotalFor(_robber1, _facelessMan));
            Assert.AreEqual(7,GetTotalFor(_cop1, _facelessMan));
        }
        
        [Test]
        public void TestRelationshipStrength_BetweenFactions()
        {
            //robbers hate cops
            _world.Relationships.Add(new InterFactionRelationship(_robbers,_cops, 5));

            //but cops hate robbers more
            _world.Relationships.Add(new InterFactionRelationship(_cops,_robbers, 7));

            //cops and robbers are all ok with their friends
            Assert.AreEqual(0,GetTotalFor(_robber1, _robber2));
            Assert.AreEqual(0,GetTotalFor(_cop1, _cop2));
       
            //todo: have to think about this one
            Assert.AreEqual(5,GetTotalFor(_turncoat, _cop2));
            Assert.AreEqual(7,GetTotalFor(_turncoat, _robber1));
            
            Assert.AreEqual(7,GetTotalFor(_cop2,_turncoat ));
            Assert.AreEqual(5,GetTotalFor(_robber1,_turncoat));

            Assert.AreEqual(5,GetTotalFor(_robber1, _cop1));
            Assert.AreEqual(7,GetTotalFor(_cop1,_robber2 ));

            Assert.AreEqual(0,GetTotalFor(_robber1, _facelessMan));
            Assert.AreEqual(0,GetTotalFor(_cop1, _facelessMan));
        }


        private double GetTotalFor(Npc observer, Npc observed)
        {
            return _world.Relationships.Where(r => r.AppliesTo(observer, observed)).Sum(r => r.Attitude);

        }
    }
}
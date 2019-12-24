﻿using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class ActorFactory : IActorFactory
    {
        public IEnumerable<IActor> Create(IPlace place)
        {
            yield return NewGuard();
        }

        private IActor NewGuard()
        {
            var g = new Actor("Guard");

            //prevents anyone leaving the room unless loyalty is 10
            g.AddBehaviour(new ForbidBehaviour<Leave>(l=>l.PerformedBy.BaseStats[Stat.Loyalty] < 10,g));

            return g;
        }
    }
}

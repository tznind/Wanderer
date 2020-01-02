﻿using System;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Behaviours
{
    public abstract class Behaviour : IBehaviour
    {
        public IHasStats Owner { get; set; }

        protected Behaviour(IHasStats owner)
        {
            Owner = owner;
        }

        public virtual void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {

        }

        public virtual void OnRoundEnding(IUserinterface ui, Guid round)
        {
        }
    }
}
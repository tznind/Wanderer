using System;
using Wanderer.Actions;
using Wanderer.Actors;

namespace Wanderer.Behaviours
{
    public abstract class Behaviour : IBehaviour
    {
        public IHasStats Owner { get; set; }

        protected Behaviour(IHasStats owner)
        {
            Owner = owner;
        }

        public virtual void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {

        }


        public virtual void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {

        }


        public virtual void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
        }

        public virtual bool AreIdentical(object other)
        {
            if (other == null)
                return false;

            //if they are different Types
            if (other.GetType() != GetType())
                return false;

            return true;
        }
    }
}
using System;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class BehaviourSystemArgs : SystemArgs
    {
        public IBehaviour Behaviour { get; set; }

        public BehaviourSystemArgs(IBehaviour behaviour, IWorld world, IUserinterface ui,  IActor aggressorIfAny, IHasStats recipient, Guid round) 
            : base(world,ui,0,aggressorIfAny, recipient, round)
        {
            Behaviour = behaviour;
        }
    }
}
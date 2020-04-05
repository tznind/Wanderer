using System;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Systems;
using Action = Wanderer.Actions.Action;

namespace Wanderer.Behaviours
{
    public class EventSystemArgs : SystemArgs
    {
        public IBehaviour Behaviour { get; set; }

        public IAction Action { get; set; }

        public EventSystemArgs(IHasStats source, IWorld world, IUserinterface ui,  IActor aggressorIfAny, IHasStats recipient, Guid round) 
            : base(world,ui,0,aggressorIfAny, recipient, round)
        {
            Behaviour = source as IBehaviour;
            Action = source as IAction;
        }
    }
}
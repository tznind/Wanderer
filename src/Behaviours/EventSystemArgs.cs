using System;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

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

        public override IHasStats GetTarget(SystemArgsTarget target)
        {
            if (target == SystemArgsTarget.Owner)
                return Behaviour.Owner;

            return base.GetTarget(target);
        }
    }
}
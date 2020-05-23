using System;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    class ActionSystemArgs : SystemArgs
    {
        public IAction Action { get; }

        public ActionSystemArgs(IAction action,IWorld world, IUserinterface ui, double intensity, IActor aggressorIfAny, IHasStats recipient, Guid round) : base(world, ui, intensity, aggressorIfAny, recipient, round)
        {
            Action = action;
        }

        public override IHasStats GetTarget(SystemArgsTarget target)
        {
            if (target == SystemArgsTarget.Owner)
                return Action.Owner;

            return base.GetTarget(target);
        }
    }
}
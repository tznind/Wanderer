using System;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    internal class LoadGunsAction : Action
    {
        public LoadGunsAction(IWorld world):base(world)
        {
            
        }

        public override void Push(IUserinterface ui, ActionStack stack)
        {
            throw new NotImplementedException();
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            throw new NotImplementedException();
        }
    }
}
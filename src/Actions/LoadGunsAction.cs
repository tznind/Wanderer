using System;

namespace StarshipWanderer.Actions
{
    internal class LoadGunsAction : Action
    {
        public LoadGunsAction(IWorld world):base(world)
        {
            
        }
        public override void Perform(IUserinterface ui)
        {
            throw new NotImplementedException();
        }
    }
}
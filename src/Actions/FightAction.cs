using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    class FightAction : Action
    {
        public FightAction(IWorld world, IActor performedBy) : base(world, performedBy)
        {
        }


        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            //remove all other occupants (wow!)
            World.CurrentLocation.Occupants.RemoveWhere(a => !(a is You));
        }
    }
}

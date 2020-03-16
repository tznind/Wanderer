using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;

namespace Wanderer.Actions
{
    public class InspectAction : Action
    {
        private InspectAction():base(null)
        {
            
        }
        public InspectAction(IHasStats owner) : base(owner)
        {
        }

        public override char HotKey => 'i';
        
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(Owner != null)
                ui.ShowStats(Owner);
        }

        public override bool HasTargets(IActor performer)
        {
            return Owner != null;
        }
        
    }
}

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
            if (actor.Decide(ui, "Inspect", null, out IActor toInspect, GetTargets(actor),0))
                ui.ShowStats(toInspect);
        }

        private IActor[] GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(true);
        }
        
        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
    }
}

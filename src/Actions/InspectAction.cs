using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;

namespace Wanderer.Actions
{
    public class InspectAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if (actor.Decide(ui, "Inspect", null, out IActor toInspect, GetTargets(actor),0))
                ui.ShowStats(toInspect);
        }

        private IActor[] GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(true);
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
    }
}

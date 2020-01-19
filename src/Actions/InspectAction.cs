using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    class InspectAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if (actor.Decide(ui, "Inspect", null, out IActor toInspect, actor.GetCurrentLocationSiblings(),0))
                ui.ShowActorStats(toInspect);
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
        }
    }
}

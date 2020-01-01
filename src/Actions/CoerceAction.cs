using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    public class CoerceAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            //pick a target 
            if(ui.GetChoice("Coerce Target", null, out Npc toCoerce, actor.GetCurrentLocationSiblings().OfType<Npc>().ToArray()))
                //pick an action to perform
                if (ui.GetChoice("Coerce Action", null, out IAction actionToCoerce,toCoerce.GetFinalActions().ToArray()))
                    stack.Push(new CoerceFrame(actor, this, toCoerce, actionToCoerce,ui));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = frame as CoerceFrame;
            f.ToCoerce.NextAction = f;

            ui.Log.Info($"{f.PerformedBy} coerced {f.ToCoerce} to perform {f.CoerceAction.Name}", stack.Round);
        }
    }
}

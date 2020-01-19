using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    public class TalkAction:Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {           
            if(actor.Decide(ui,"Talk To","Pick target",out IActor chosen,
            actor.CurrentLocation.World.Dialogue.GetAvailableTalkTargets(actor).ToArray(),0))
                stack.Push(new Frame(actor,this,0){TargetIfAny = chosen});
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            //apply the dialogue system
            frame.PerformedBy.CurrentLocation.World.Dialogue.Apply(new SystemArgs(ui, 0, frame.PerformedBy,
                frame.TargetIfAny, stack.Round));
        }
    }
}

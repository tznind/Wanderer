using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    public class Coerce : Action
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



        }
    }

    public class CoerceFrame : Frame
    {
        public Npc ToCoerce { get; }
        public IAction CoerceAction { get; }
        public IUserinterface UserInterface { get; }

        /// <summary>
        /// This frame is passed to the <see cref="Npc.NextAction"/> once they have chosen the action this becomes true
        /// </summary>
        public bool Chosen { get; set; }

        public CoerceFrame(IActor performedBy, Coerce action, Npc toCoerce, IAction coerceAction, IUserinterface ui):base(performedBy,action)
        {
            ToCoerce = toCoerce;
            CoerceAction = coerceAction;
            UserInterface = ui;
        }
    }
}

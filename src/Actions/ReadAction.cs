using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    public class ReadAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Read","Item to read?",out IItem selected,actor.Items.Where(i=>i.NextDialogue.HasValue).ToArray(),0))
                stack.Push(new ReadFrame(actor,this,0,selected));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (ReadFrame) frame;
            var d = f.PerformedBy.CurrentLocation.World.Dialogue;
            
            var dialogueNode = d.GetDialogue(f.Item.NextDialogue);
            
            if(dialogueNode != null)
                d.Run(new SystemArgs(ui,0,null,f.PerformedBy,stack.Round),dialogueNode );
        }
    }
}
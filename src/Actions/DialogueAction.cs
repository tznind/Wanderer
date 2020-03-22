using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class DialogueAction:Action
    {

        public DialogueAction(IHasStats owner):base(owner)
        {
            Name = "Other";
            HotKey = 'o';
        }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(ValidatePick(actor,Owner,out string reason))
                stack.Push(new DialogueFrame(actor,this,Owner,0));
            else
                ui.ShowMessage("Not Possible", reason);
        }
        
        private bool ValidatePick(IActor actor, IHasStats target,out string reason)
        {
            if (target is IItem i && !i.CanUse(actor, out reason))
                return false;

            reason = null;
            return true;
        }

        
        protected override void PopImpl(IWorld world1, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (DialogueFrame) frame;
            var world = frame.PerformedBy.CurrentLocation.World;
            //apply the dialogue system
            world.Dialogue.Apply(new SystemArgs(world,ui, 0, f.PerformedBy, f.DialogueTarget, stack.Round));
        }

        public override bool HasTargets(IActor performer)
        {
            return Owner?.Dialogue.Next != null;
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if(Owner?.Dialogue != null)
                yield return Owner;
        }

        public override string ToString()
        {
            if(Owner?.Dialogue!= null)
                return Owner.Dialogue.Verb + ":" + Owner.Name;

            return base.ToString();
        }
    }
}

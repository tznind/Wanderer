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

        public override char HotKey => 'o';

        private DialogueAction():base(null)
        {
            Name = "Other";
        }

        public DialogueAction(IHasStats owner):base(owner)
        {
            Name = "Other";
        }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            var targets = GetTargets(actor);
            
            if(actor.Decide(ui,"Talk To","Pick target",out string chosen, targets.Keys.ToArray(),0))
                if(ValidatePick(actor,targets[chosen],out string reason))
                    stack.Push(new DialogueFrame(actor,this,targets[chosen],0));
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


        public override void Pop(IWorld world1, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (DialogueFrame) frame;
            var world = frame.PerformedBy.CurrentLocation.World;
            //apply the dialogue system
            world.Dialogue.Apply(new SystemArgs(world,ui, 0, f.PerformedBy, f.DialogueTarget, stack.Round));
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
        private Dictionary<string, IHasStats> GetTargets(IActor actor)
        {
            var targets = new Dictionary<string, IHasStats>();

            AddTarget(actor,targets, actor.CurrentLocation);

            foreach (var a in actor.GetCurrentLocationSiblings(false))
                AddTarget(actor,targets, a);
            
            foreach (var i in actor.Items) 
                AddTarget(actor,targets, i);

            return targets;
        }
        private void AddTarget(IActor actor, Dictionary<string, IHasStats> targets, IHasStats possibleTarget)
        {
            if(possibleTarget == null || possibleTarget.Dialogue.IsEmpty)
                return;
            
            string option = possibleTarget.Dialogue.Verb + ":" + possibleTarget.Name;

            if(!targets.ContainsKey(option))
                targets.Add(option,possibleTarget);
            else
            {
                int duplicate = 2;
                while (targets.ContainsKey(option + duplicate)) 
                    duplicate++;
                
                targets.Add(option + duplicate,possibleTarget);
            }
        }
    }
}

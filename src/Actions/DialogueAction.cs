using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    public class DialogueAction:Action
    {
        public DialogueAction()
        {
            Name = "Other";
        }
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            var targets = new Dictionary<string, IHasStats>();

            AddTarget(targets, actor.CurrentLocation);

            foreach (var a in actor.GetCurrentLocationSiblings())
                AddTarget(targets, a);
            
            foreach (var i in actor.Items) 
                AddTarget(targets, i);
            
            if(actor.Decide(ui,"Talk To","Pick target",out string chosen, targets.Keys.ToArray(),0))
                stack.Push(new DialogueFrame(actor,this,targets[chosen],0));
        }

        private void AddTarget(Dictionary<string, IHasStats> targets, IHasStats possibleTarget)
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

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (DialogueFrame) frame;
            //apply the dialogue system
            frame.PerformedBy.CurrentLocation.World.Dialogue.Apply(new SystemArgs(ui, 0, f.PerformedBy, f.DialogueTarget, stack.Round));
        }
    }
}

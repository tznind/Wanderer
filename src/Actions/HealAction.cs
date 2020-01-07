using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Actions
{
    public class HealAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Heal","Choose who to heal",out IActor target, actor.CurrentLocation.Actors.Where(a => a.Has<Injured>(false)).ToArray(),10))
                if(actor.Decide(ui,"Injury", "Choose an Injury",out Injured toHeal, target.Adjectives.OfType<Injured>().ToArray(),10))
                    stack.Push(new HealFrame(actor,this,target,toHeal));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (HealFrame)frame;

            if (f.ToBeHealed.Adjectives.Contains(f.Injury)) 
                f.Injury.Heal(ui, stack.Round);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Plans;

namespace Wanderer.Actions
{
    public class LeadershipAction : Action
    {
        public override char HotKey => 'a';

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(actor.Decide(ui,"Leadership",null,out IActor chosen,GetTargets(actor).ToArray(),0))
                if(actor.Decide(ui,"Plan",$"Pick a plan for {chosen.Name} to prioritize",out Plan plan,GetPlans(world,stack,actor).ToArray(),0))
                    if(actor.Decide(ui,"Priority",$"Set a priority for plan {plan}",out double weight,new double[]{-30,-20,-10,0,10,20,30,50,100},0))
                    {
                        stack.Push(new LeadershipFrame(actor,this,chosen,plan,weight));
                    }
        }

        protected virtual IEnumerable<Plan> GetPlans(IWorld world, ActionStack stack, IActor actor)
        {
            foreach (var plan in world.PlanningSystem.Plans)
                yield return plan;
            
            
            yield return new FollowPlan(actor){Name = "Follow Me"};
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (LeadershipFrame)frame;
            var led = new LedAdjective(f);

            //get rid of any old leadership effects for that plan
            foreach(var a in f.TargetIfAny.Adjectives.OfType<LedAdjective>().Where(l=>l.Led.Plan == f.Plan).ToArray())
                f.TargetIfAny.Adjectives.Remove(a);
            
            //TODO: set expiry to be relative to Leadership
            f.TargetIfAny.Adjectives.Add(led.WithExpiry(10));

        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        public IEnumerable<IActor> GetTargets(IActor performer)
        {
            //return people who view you in a positive light
            return performer.GetCurrentLocationSiblings(false)
            .Where(
                a=>
                performer.CurrentLocation.World.Relationships.SumBetween(a,performer) > 0
            );
        }

    }
}
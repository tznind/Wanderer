using Wanderer.Actors;
using Wanderer.Plans;

namespace Wanderer.Actions
{
    public class LeadershipFrame : Frame
    {
        public Plan Plan { get; set;}
        public double Weight { get; set;}

        public LeadershipFrame(IActor performedBy,IAction action,IActor target, Plan plan, double weight):base(performedBy,action,0)
        {
            TargetIfAny = target;
            Plan = plan;
            Weight = weight;
        }
    }
}
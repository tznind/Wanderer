using Wanderer.Actors;
using Wanderer.Adjectives.ActorOnly;

namespace Wanderer.Actions
{
    public class HealFrame : Frame
    {
        public Injured Injury { get; }

        public HealFrame(IActor healer, IAction action, IActor toBeHealed, Injured injury,double attitude) : base(healer,action,attitude)
        {
            TargetIfAny = toBeHealed;
            Injury = injury;
        }
    }
}
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Actions
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
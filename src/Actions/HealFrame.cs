using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Actions
{
    public class HealFrame : Frame
    {
        public Injured Injury { get; }

        public HealFrame(IActor healer, IAction action, IActor toBeHealed, Injured injury) : base(healer,action)
        {
            TargetIfAny = toBeHealed;
            Injury = injury;
        }
    }
}
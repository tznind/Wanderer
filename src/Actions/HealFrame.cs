using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Actions
{
    public class HealFrame : Frame
    {
        public IActor ToBeHealed { get; }
        public Injured Injury { get; }

        public HealFrame(IActor healer, IAction action, IActor toBeHealed, Injured injury) : base(healer,action)
        {
            ToBeHealed = toBeHealed;
            Injury = injury;
        }
    }
}
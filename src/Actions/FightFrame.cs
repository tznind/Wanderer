using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    internal class FightFrame : Frame
    {
        public IActor FightTarget { get; }

        public FightFrame(IActor performedBy,IActor fightTarget, IAction action) : base(performedBy, action)
        {
            FightTarget = fightTarget;
        }
    }
}
using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    internal class FightFrame : Frame
    {
        public IInjurySystem InjurySystem { get; }

        public FightFrame(IActor performedBy,IActor fightTarget, IAction action, IInjurySystem injurySystem) : base(performedBy, action)
        {
            TargetIfAny = fightTarget;
            InjurySystem = injurySystem;
        }
    }
}
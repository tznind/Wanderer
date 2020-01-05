using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    internal class FightFrame : Frame
    {
        public IActor FightTarget { get; }

        public IInjurySystem InjurySystem { get; }

        public FightFrame(IActor performedBy,IActor fightTarget, IAction action, IInjurySystem injurySystem) : base(performedBy, action)
        {
            FightTarget = fightTarget;
            InjurySystem = injurySystem;
        }
    }
}
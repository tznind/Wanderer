using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class FightFrame : Frame
    {
        public IInjurySystem InjurySystem { get; }

        public FightFrame(IActor performedBy,IActor fightTarget, IAction action, IInjurySystem injurySystem,double attitude) : base(performedBy, action,attitude)
        {
            TargetIfAny = fightTarget;
            InjurySystem = injurySystem;
        }
    }
}
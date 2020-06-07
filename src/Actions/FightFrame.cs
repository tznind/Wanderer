using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class FightFrame : Frame
    {
        public IInjurySystem InjurySystem { get; set; }

        public FightFrame(IActor performedBy,IActor fightTarget, IAction action, IInjurySystem injurySystem,double attitude) : base(performedBy, action,attitude)
        {
            TargetIfAny = fightTarget;
            InjurySystem = injurySystem;
        }

        ActionTarget ToFight {get;set;}

        public override ActionTarget GetNextRequiredTarget()
        {
            if(ToFight == null)
                return ToFight = new ActionTarget(PerformedBy.GetCurrentLocationSiblings(false),"Target to fight",-20);

            return null;
        }
    }
}
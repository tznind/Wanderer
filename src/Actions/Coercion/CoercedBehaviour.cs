using Wanderer.Behaviours;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actions.Coercion
{
    /// <summary>
    /// Controls the duration and cancellation (based on exactly what you are asking
    /// them to do) of the <see cref="Coerced"/> adjective.
    /// </summary>
    public class CoercedBehaviour : ExpiryBehaviour
    {
        public Coerced Coerced { get; }

        public CoercedBehaviour(Coerced coerced):base(coerced,1)
        {
            Coerced = coerced;
        }

        public override void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
            base.OnPush(ui, stack, frame);
            
            var coerceFrame = Coerced.CoercedFrame;

            //if the coerced frame has had all it's decisions made and is about to be pushed onto the stack
            if (ReferenceEquals(frame.Action,coerceFrame.CoerceAction))
            {
                var args = new NegotiationSystemArgs(ui, coerceFrame.PerformedBy.GetFinalStats()[Stat.Coerce],
                    coerceFrame.PerformedBy, coerceFrame.TargetIfAny, stack.Round);
                
                //the final frame that is being pushed is what we are proposing they do
                args.Proposed = frame;

                //Check with the negotiation system to see if the actor is still willing to go ahead
                //with the action as configured
                Coerced.CoercedFrame.NegotiationSystem.Apply(args);

                //if not then cancel
                if (!args.Willing)
                {
                    var narrative = new Narrative(coerceFrame.PerformedBy, "Coerce Failed",
                        $"{coerceFrame.TargetIfAny} refused to perform action",
                        $"{coerceFrame.PerformedBy} failed to coerce {coerceFrame.TargetIfAny} - {args.WillingReason}", stack.Round);

                    narrative.Show(ui);

                    frame.Cancelled = true;
                }
            }
        }
    }
}
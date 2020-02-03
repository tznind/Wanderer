using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions.Coercion
{
    public class CoerceFrame : Frame
    {
        public IAction CoerceAction { get; }
        public IUserinterface UserInterface { get; }
        public INegotiationSystem NegotiationSystem { get; }

        /// <summary>
        /// This frame is passed to the <see cref="Npc.NextAction"/> once they have chosen the action this becomes true
        /// </summary>
        public bool Chosen { get; set; }

        public CoerceFrame(IActor performedBy, CoerceAction action, Npc toCoerce, IAction coerceAction,
            IUserinterface ui, INegotiationSystem negotiationSystem, double attitude):base(performedBy,action,attitude)
        {
            TargetIfAny = toCoerce;
            CoerceAction = coerceAction;
            UserInterface = ui;
            NegotiationSystem = negotiationSystem;
        }
    }
}
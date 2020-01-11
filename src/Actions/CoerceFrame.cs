using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    public class CoerceFrame : Frame
    {
        public IAction CoerceAction { get; }
        public IUserinterface UserInterface { get; }

        /// <summary>
        /// This frame is passed to the <see cref="Npc.NextAction"/> once they have chosen the action this becomes true
        /// </summary>
        public bool Chosen { get; set; }

        public CoerceFrame(IActor performedBy, CoerceAction action, Npc toCoerce, IAction coerceAction, IUserinterface ui):base(performedBy,action)
        {
            TargetIfAny = toCoerce;
            CoerceAction = coerceAction;
            UserInterface = ui;
        }
    }
}
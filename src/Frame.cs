using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    public class Frame
    {
        public IActor PerformedBy { get; set; }
        public IAction Action { get; set; }
        public CancellationStatus Cancelled { get; set; } = CancellationStatus.NotCancelled;

        public Frame(IActor performedBy, IAction action)
        {
            PerformedBy = performedBy;
            Action = action;
        }

    }
}
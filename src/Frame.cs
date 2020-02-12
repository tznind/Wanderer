using Wanderer.Actions;
using Wanderer.Actors;

namespace Wanderer
{
    public class Frame
    {
        /// <summary>
        /// How hostile/friendly the action is.  Positive for kind things, negative for hostile
        /// </summary>
        public double Attitude { get; set; }

        public IActor PerformedBy { get; set; }

        public IActor TargetIfAny { get; set; }

        public IAction Action { get; set; }
        public bool Cancelled { get; set; }

        public Frame(IActor performedBy, IAction action,double attitude)
        {
            PerformedBy = performedBy;
            Action = action;
            Attitude = attitude;
        }

    }
}
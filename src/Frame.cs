using System;
using System.Linq;
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

        /// <summary> 
        /// Returns the origin of <see cref="Action"/> or null if a frame was generated
        /// which somehow did not originate from the <see cref="PerformedBy"/> or one
        /// of his child items
        /// <summary/>
        internal IHasStats GetActionOwner()
        {
            return PerformedBy.GetAllHaves().FirstOrDefault(h=>
            h.GetFinalActions(PerformedBy).Contains(Action));
        }
    }
}
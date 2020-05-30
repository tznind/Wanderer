using System;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;

namespace Wanderer
{
    /// <summary>
    /// Describes an <see cref="IAction"/> and the player (or NPC) selected targets made immediately before it was pushed onto the <see cref="ActionStack"/>
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// How hostile/friendly the action is.  Positive for kind things, negative for hostile
        /// </summary>
        public double Attitude { get; set; }

        /// <summary>
        /// The actor performing the <see cref="Action"/>
        /// </summary>
        public IActor PerformedBy { get; set; }

        /// <summary>
        /// If the <see cref="Action"/> has a single or primary
        /// target, this is what was targeted
        /// </summary>
        public IHasStats TargetIfAny { get; set; }

        /// <summary>
        /// The action being <see cref="PerformedBy"/> someone.  Note that the Owner of the action may be an item or room while the <see cref="PerformedBy"/> is someone else
        /// </summary>
        public IAction Action { get; set; }

        /// <summary>
        /// True if some point after being Pushed onto an <see cref="ActionStack"/> the frame was cancelled (before Pop)
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Creates a new instance recording that a given <paramref name="action"/> is being attempted
        /// </summary>
        /// <param name="performedBy">The actor performing the action</param>
        /// <param name="action">The action being performed</param>
        /// <param name="attitude">How kind the or unkind the act is (affects relationships, changes AI etc)</param>
        public Frame(IActor performedBy, IAction action,double attitude)
        {
            PerformedBy = performedBy;
            Action = action;
            Attitude = attitude;
        }
    }
}
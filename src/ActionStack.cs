using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer
{
    /// <summary>
    /// Handles resolving <see cref="IAction"/> and response actions triggered (e.g. by <see cref="IBehaviour"/>)
    /// </summary>
    public class ActionStack : Stack<Frame>
    {
        public Guid Round { get; }= Guid.NewGuid();

        /// <summary>
        /// Runs the <paramref name="firstAction"/> and evaluates all responders
        /// </summary>
        /// <param name="ui">When decisions require user input, this handles it</param>
        /// <param name="firstAction">The initial action (to go on bottom of stack)</param>
        /// <param name="performer">Who is attempting <paramref name="firstAction"/></param>
        /// <param name="responders">All valid responders</param>
        public bool RunStack(IUserinterface ui, IAction firstAction,IActor performer, IEnumerable<IBehaviour> responders)
        {
            //and run push event on the action
            firstAction.Push(ui,this,performer);

            //If the action decided not to push after all (e.g. UI cancel or decision not to push)
            if (this.Count == 0)
                return false; //initial action was aborted

            //check all behaviours to see if they want to respond (by pushing actions etc)
            foreach (IBehaviour responder in responders.ToArray())  //ToArray needed because they can self destruct at this time!
                responder.OnPush(ui, this,Peek());

            var pending = new Queue<Frame>();

            //run all tasks that are not pending cancellation
            while(TryPop(out Frame current))
                switch (current.Cancelled)
                {
                    case CancellationStatus.NotCancelled:
                        current.Action.Pop(ui,this, current);
                        break;
                    case CancellationStatus.CancellationPending:
                        pending.Enqueue(current);
                        break;
                    case CancellationStatus.Cancelled:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            //now run any actions we thought were going to be cancelled but weren't after all
            while(pending.TryDequeue(out Frame current))
                if(current.Cancelled != CancellationStatus.Cancelled)
                    current.Action.Pop(ui,this, current);

            Clear();

            return true;
        }
    }

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